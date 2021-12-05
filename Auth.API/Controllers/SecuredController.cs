using Duende.IdentityServer.ResponseHandling;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Headers;

namespace AuthAPI.Controllers;

//[Authorize]
[Route("api/[controller]")]
[ApiController]
public class SecuredController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public SecuredController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet("GetToken")]
    public async Task<IActionResult> GetToken()
    {
        var discoveryClient = _httpClientFactory.CreateClient();
        var discoveryDocument = await discoveryClient.GetDiscoveryDocumentAsync("https://localhost:44307/");

        var tokenResponse = await discoveryClient.RequestClientCredentialsTokenAsync(
            new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,

                ClientId = "client_id",
                ClientSecret = "client_secret",

                Scope = "SecretApi"
            });

        return Ok(tokenResponse);
    }

    [HttpGet("GetSecuredData")]
    public async Task<IActionResult> GetSecuredData()
    {
        var discoveryClient = _httpClientFactory.CreateClient();
        var discoveryDocument = await discoveryClient.GetDiscoveryDocumentAsync("https://localhost:44307/");

        var tokenResponse = await discoveryClient.RequestClientCredentialsTokenAsync(
            new ClientCredentialsTokenRequest
            {
                Address = discoveryDocument.TokenEndpoint,

                ClientId = "client_id",
                ClientSecret = "client_secret",

                Scope = "SecretApi"
            });

        var secretApiClient = _httpClientFactory.CreateClient();
        secretApiClient.SetBearerToken(tokenResponse.AccessToken);

        var response = await secretApiClient.GetAsync("https://localhost:7283/api/secret");

        string result = "";

        if (response.IsSuccessStatusCode)
            result = await response.Content.ReadAsStringAsync();
        else
            result = response.ReasonPhrase;

        return new OkObjectResult(result);
    }

    [HttpPost("PostSecuredData")]
    public IActionResult PostSecuredData() =>
        Ok("This Secured Data is available only for Authenticated Users.");
}
