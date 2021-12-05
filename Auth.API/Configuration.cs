using Duende.IdentityServer.Models;
using IdentityModel;

namespace Auth.API;
public static class Configuration
{
    public static IEnumerable<Client> GetClients() =>
        new List<Client>
        {
            new Client()
            {
                ClientId = "client_id",
                ClientSecrets =  new List<Secret>
                {
                    new Secret("client_secret".ToSha256())
                },
                AllowedGrantTypes = GrantTypes.ClientCredentials,
                AllowedScopes = new List<string>
                {
                    "SecretApi"
                },
            },
        };

    public static IEnumerable<ApiResource> GetApiResources() =>
        new List<ApiResource>
        {
            new ApiResource()
            {
                Name = "SecretApi",
                Scopes = {"SecretApi"},
            }
        };

    public static IEnumerable<ApiScope> GetApiScopes() =>
        new List<ApiScope>
        {
            new ApiScope("SecretApi")
        };

    public static IEnumerable<IdentityResource> GetIdentityResources() =>
        new List<IdentityResource>
        {
        new IdentityResources.OpenId(),
        };
}