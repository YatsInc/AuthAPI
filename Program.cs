using AuthAPI;
using AuthAPI.Data;
using AuthAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration["ConnectionStrings:Postgresql"];

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(o =>
{
    o.UseNpgsql(connectionString);
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

builder.Services.AddHttpClient();

builder.Services.AddIdentityServer()
    .AddInMemoryClients(Configuration.GetClients())
    .AddInMemoryApiResources(Configuration.GetApiResources())
    .AddInMemoryApiScopes(Configuration.GetApiScopes())
    .AddInMemoryIdentityResources(Configuration.GetIdentityResources())
    .AddDeveloperSigningCredential();

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

app.UseAuthentication();

app.UseAuthorization();

app.UseIdentityServer();

app.MapControllers();

app.Run();