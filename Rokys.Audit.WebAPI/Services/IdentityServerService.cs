using System.Security.Claims;

namespace Rokys.Audit.WebAPI.Services
{
    public interface IIdentityServerService
    {
        Task<bool> ValidateTokenAsync(string token);
        Task<ClaimsPrincipal?> GetUserClaimsAsync(string token);
        Task<bool> HasPermissionAsync(string userId, string permission, string resource);
    }

    public class IdentityServerService : IIdentityServerService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<IdentityServerService> _logger;

        public IdentityServerService(HttpClient httpClient, IConfiguration configuration, ILogger<IdentityServerService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            
            var authority = _configuration["IdentityServer:Authority"];
            if (!string.IsNullOrEmpty(authority))
            {
                _httpClient.BaseAddress = new Uri(authority);
            }
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var introspectEndpoint = _configuration["IdentityServer:Authority"]?.TrimEnd('/') + "/connect/introspect";
                
                var formParams = new List<KeyValuePair<string, string>>
                {
                    new("token", token),
                    new("client_id", _configuration["IdentityServer:ClientId"] ?? "rokys-memo-api"),
                    new("client_secret", _configuration["IdentityServer:ClientSecret"] ?? "")
                };

                var formContent = new FormUrlEncodedContent(formParams);
                var response = await _httpClient.PostAsync(introspectEndpoint, formContent);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return content.Contains("\"active\":true");
                }

                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token with IdentityServer");
                return false;
            }
        }

        public async Task<ClaimsPrincipal?> GetUserClaimsAsync(string token)
        {
            try
            {
                var introspectEndpoint = _configuration["IdentityServer:Authority"]?.TrimEnd('/') + "/connect/introspect";
                
                var formParams = new List<KeyValuePair<string, string>>
                {
                    new("token", token),
                    new("client_id", _configuration["IdentityServer:ClientId"] ?? "rokys-memo-api"),
                    new("client_secret", _configuration["IdentityServer:ClientSecret"] ?? "")
                };

                var formContent = new FormUrlEncodedContent(formParams);
                var response = await _httpClient.PostAsync(introspectEndpoint, formContent);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    
                    // Parse the introspection response and create claims
                    // This is a simplified implementation - you might want to use a JSON library
                    var claims = new List<Claim>();
                    
                    // Add basic claims from the response
                    if (content.Contains("\"active\":true"))
                    {
                        claims.Add(new Claim("active", "true"));
                        
                        // Extract other claims as needed
                        // You can enhance this to parse JSON properly
                    }

                    return new ClaimsPrincipal(new ClaimsIdentity(claims, "Bearer"));
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user claims from IdentityServer");
                return null;
            }
        }

        public async Task<bool> HasPermissionAsync(string userId, string permission, string resource)
        {
            try
            {
                // Implement your business logic for checking permissions
                // This could involve calling your local database or another service
                
                _logger.LogInformation($"Checking permission {permission} for user {userId} on resource {resource}");
                
                // For now, return true - implement your actual permission logic here
                return await Task.FromResult(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking permission {permission} for user {userId}");
                return false;
            }
        }
    }
}