using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace Rokys.Audit.WebAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosticsController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DiagnosticsController> _logger;

        public DiagnosticsController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<DiagnosticsController> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("identity-server-discovery")]
        public async Task<IActionResult> TestIdentityServerDiscovery()
        {
            try
            {
                var authority = _configuration["IdentityServer:Authority"];
                if (string.IsNullOrEmpty(authority))
                {
                    return BadRequest("IdentityServer Authority not configured");
                }

                var discoveryUrl = $"{authority.TrimEnd('/')}/.well-known/oauth-authorization-server";
                _logger.LogInformation("Testing IdentityServer discovery at: {Url}", discoveryUrl);

                var response = await _httpClient.GetAsync(discoveryUrl);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Parse and format the JSON for better readability
                    var jsonDocument = JsonDocument.Parse(content);
                    var formattedJson = JsonSerializer.Serialize(jsonDocument, new JsonSerializerOptions 
                    { 
                        WriteIndented = true 
                    });

                    return Ok(new
                    {
                        Status = "Success",
                        StatusCode = response.StatusCode,
                        DiscoveryUrl = discoveryUrl,
                        Configuration = JsonSerializer.Deserialize<object>(formattedJson)
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        Status = "Failed",
                        StatusCode = response.StatusCode,
                        DiscoveryUrl = discoveryUrl,
                        Error = content
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing IdentityServer discovery");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message,
                    StackTrace = ex.StackTrace
                });
            }
        }

        [HttpGet("jwks")]
        public async Task<IActionResult> TestJwksEndpoint()
        {
            try
            {
                var authority = _configuration["IdentityServer:Authority"];
                if (string.IsNullOrEmpty(authority))
                {
                    return BadRequest("IdentityServer Authority not configured");
                }

                var jwksUrl = $"{authority.TrimEnd('/')}/.well-known/jwks";
                _logger.LogInformation("Testing JWKS endpoint at: {Url}", jwksUrl);

                var response = await _httpClient.GetAsync(jwksUrl);
                var content = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var jsonDocument = JsonDocument.Parse(content);
                    var formattedJson = JsonSerializer.Serialize(jsonDocument, new JsonSerializerOptions 
                    { 
                        WriteIndented = true 
                    });

                    return Ok(new
                    {
                        Status = "Success",
                        StatusCode = response.StatusCode,
                        JwksUrl = jwksUrl,
                        Keys = JsonSerializer.Deserialize<object>(formattedJson)
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        Status = "Failed",
                        StatusCode = response.StatusCode,
                        JwksUrl = jwksUrl,
                        Error = content
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error testing JWKS endpoint");
                return StatusCode(500, new
                {
                    Status = "Error",
                    Message = ex.Message
                });
            }
        }

        [HttpGet("configuration")]
        public IActionResult GetCurrentConfiguration()
        {
            return Ok(new
            {
                IdentityServer = new
                {
                    Authority = _configuration["IdentityServer:Authority"],
                    Audience = _configuration["IdentityServer:Audience"],
                    ClientId = _configuration["IdentityServer:ClientId"],
                    RequireHttpsMetadata = _configuration["IdentityServer:RequireHttpsMetadata"]
                },
                Security = new
                {
                    Enabled = _configuration["Security:Enabled"]
                }
            });
        }
    }
}