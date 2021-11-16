using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration["ConnectionStrings:Postgresql"];

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(o =>
{
    o.UseNpgsql(connectionString);
    o.UseOpenIddict();
});

// Register the Identity services.
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder
                .AllowCredentials()
                .WithOrigins(
                    "https://localhost:4200")
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});
// Configure Identity to use the same JWT claims as OpenIddict instead
// of the legacy WS-Federation claims it uses by default (ClaimTypes),
// which saves you from doing the mapping in your authorization controller.
builder.Services.Configure<IdentityOptions>(options =>
{
    options.ClaimsIdentity.UserNameClaimType = Claims.Name;
    options.ClaimsIdentity.UserIdClaimType = Claims.Subject;
    options.ClaimsIdentity.RoleClaimType = Claims.Role;
});

// OpenIddict offers native integration with Quartz.NET to perform scheduled tasks
// (like pruning orphaned authorizations/tokens from the database) at regular intervals.
builder.Services.AddQuartz(options =>
{
    options.UseMicrosoftDependencyInjectionJobFactory();
    options.UseSimpleTypeLoader();
    options.UseInMemoryStore();
});

// Register the Quartz.NET service and configure it to block shutdown until jobs are complete.
builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

builder.Services.AddOpenIddict()

    // Register the OpenIddict core components.
    .AddCore(options =>
    {
            // Configure OpenIddict to use the Entity Framework Core stores and models.
            // Note: call ReplaceDefaultEntities() to replace the default OpenIddict entities.
            options.UseEntityFrameworkCore()
               .UseDbContext<ApplicationDbContext>();

            // Developers who prefer using MongoDB can remove the previous lines
            // and configure OpenIddict to use the specified MongoDB database:
            // options.UseMongoDb()
            //        .UseDatabase(new MongoClient().GetDatabase("openiddict"));

            // Enable Quartz.NET integration.
            options.UseQuartz();
    })

    // Register the OpenIddict server components.
    .AddServer(options =>
    {
            // Enable the authorization, device, logout, token, userinfo and verification endpoints.
            options.SetAuthorizationEndpointUris("/connect/authorize")
               .SetDeviceEndpointUris("/connect/device")
               .SetLogoutEndpointUris("/connect/logout")
               .SetIntrospectionEndpointUris("/connect/introspect")
               .SetTokenEndpointUris("/connect/token")
               .SetUserinfoEndpointUris("/connect/userinfo")
               .SetVerificationEndpointUris("/connect/verify");

            // Note: this sample uses the code, device code, password and refresh token flows, but you
            // can enable the other flows if you need to support implicit or client credentials.
            options.AllowAuthorizationCodeFlow()
               .AllowDeviceCodeFlow()
               .AllowHybridFlow()
               .AllowRefreshTokenFlow();

            // Mark the "email", "profile", "roles" and "dataEventRecords" scopes as supported scopes.
            options.RegisterScopes(Scopes.Email, Scopes.Profile, Scopes.Roles, "dataEventRecords");

            // Register the signing and encryption credentials.
            options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();

            // Force client applications to use Proof Key for Code Exchange (PKCE).
            options.RequireProofKeyForCodeExchange();

            // Register the ASP.NET Core host and configure the ASP.NET Core-specific options.
            options.UseAspNetCore()
               .EnableStatusCodePagesIntegration()
               .EnableAuthorizationEndpointPassthrough()
               .EnableLogoutEndpointPassthrough()
               .EnableTokenEndpointPassthrough()
               .EnableUserinfoEndpointPassthrough()
               .EnableVerificationEndpointPassthrough()
               .DisableTransportSecurityRequirement(); // During development, you can disable the HTTPS requirement.

            // Note: if you don't want to specify a client_id when sending
            // a token or revocation request, uncomment the following line:
            //
            // options.AcceptAnonymousClients();

            // Note: if you want to process authorization and token requests
            // that specify non-registered scopes, uncomment the following line:
            //
            // options.DisableScopeValidation();

            // Note: if you don't want to use permissions, you can disable
            // permission enforcement by uncommenting the following lines:
            //
            // options.IgnoreEndpointPermissions()
            //        .IgnoreGrantTypePermissions()
            //        .IgnoreResponseTypePermissions()
            //        .IgnoreScopePermissions();

            // Note: when issuing access tokens used by third-party APIs
            // you don't own, you can disable access token encryption:
            //
            // options.DisableAccessTokenEncryption();
        })

    // Register the OpenIddict validation components.
    .AddValidation(options =>
    {
            // Configure the audience accepted by this resource server.
            // The value MUST match the audience associated with the
            // "demo_api" scope, which is used by ResourceController.
            options.AddAudiences("rs_dataEventRecordsApi");

            // Import the configuration from the local OpenIddict server instance.
            options.UseLocalServer();

            // Register the ASP.NET Core host.
            options.UseAspNetCore();

            // For applications that need immediate access token or authorization
            // revocation, the database entry of the received tokens and their
            // associated authorizations can be validated for each API call.
            // Enabling these options may have a negative impact on performance.
            //
            // options.EnableAuthorizationEntryValidation();
            // options.EnableTokenEntryValidation();
        });

builder.Services.AddControllers();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Auth.API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Auth.API v1"));
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();