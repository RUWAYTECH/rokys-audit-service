using Microsoft.IdentityModel.Tokens;
using Microsoft.IdentityModel.JsonWebTokens;
using System.Security.Claims;
using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Rokys.Audit.WebAPI.Services
{
    public class CustomJwtSecurityTokenHandler
    {
        private readonly ILogger<CustomJwtSecurityTokenHandler> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        public CustomJwtSecurityTokenHandler(
            ILogger<CustomJwtSecurityTokenHandler> logger,
            IConfiguration configuration,
            HttpClient httpClient)
        {
            _logger = logger;
            _configuration = configuration;
            _httpClient = httpClient;
        }

        public async Task<ClaimsPrincipal> ValidateTokenAsync(string token)
        {
            try
            {
                var handler = new JsonWebTokenHandler();
                
                // Obtener las claves JWKS
                var signingKeys = await GetSigningKeysAsync();
                
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = false, // Temporalmente deshabilitado para testing
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _configuration["IdentityServer:Authority"],
                    ValidAudience = _configuration["IdentityServer:Audience"],
                    IssuerSigningKeys = signingKeys,
                    ClockSkew = TimeSpan.FromMinutes(5)
                };

                var result = await handler.ValidateTokenAsync(token, validationParameters);
                
                if (result.IsValid)
                {
                    _logger.LogInformation("Token validated successfully");
                    return new ClaimsPrincipal(result.ClaimsIdentity);
                }
                else
                {
                    _logger.LogError("Token validation failed: {Exception}", result.Exception?.Message);
                    throw new SecurityTokenValidationException("Token validation failed", result.Exception);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating JWT token");
                throw;
            }
        }

        private async Task<IEnumerable<SecurityKey>> GetSigningKeysAsync()
        {
            try
            {
                var authority = _configuration["IdentityServer:Authority"]?.TrimEnd('/');
                var jwksUri = $"{authority}/.well-known/jwks";
                
                _logger.LogInformation("Fetching JWKS from: {JwksUri}", jwksUri);
                
                var response = await _httpClient.GetStringAsync(jwksUri);
                var jwks = JsonConvert.DeserializeObject<JwksResponse>(response);
                
                var keys = new List<SecurityKey>();
                
                foreach (var key in jwks.Keys)
                {
                    if (key.Kty == "RSA")
                    {
                        var rsa = RSA.Create();
                        rsa.ImportParameters(new RSAParameters
                        {
                            Modulus = Base64UrlEncoder.DecodeBytes(key.N),
                            Exponent = Base64UrlEncoder.DecodeBytes(key.E)
                        });
                        
                        var rsaKey = new RsaSecurityKey(rsa)
                        {
                            KeyId = key.Kid
                        };
                        
                        keys.Add(rsaKey);
                        _logger.LogInformation("Added RSA key with kid: {Kid}", key.Kid);
                    }
                }
                
                return keys;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching or parsing JWKS");
                throw;
            }
        }
    }

    public class JwksResponse
    {
        [JsonProperty("keys")]
        public List<JwkKey> Keys { get; set; } = new List<JwkKey>();
    }

    public class JwkKey
    {
        [JsonProperty("kty")]
        public string Kty { get; set; }

        [JsonProperty("use")]
        public string Use { get; set; }

        [JsonProperty("kid")]
        public string Kid { get; set; }

        [JsonProperty("alg")]
        public string Alg { get; set; }

        [JsonProperty("n")]
        public string N { get; set; }

        [JsonProperty("e")]
        public string E { get; set; }
    }
}