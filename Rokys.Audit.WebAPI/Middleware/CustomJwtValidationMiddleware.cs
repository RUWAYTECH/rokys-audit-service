using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Rokys.Audit.WebAPI.Services;

namespace Rokys.Audit.WebAPI.Middleware
{
    public class CustomJwtValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomJwtValidationMiddleware> _logger;

        public CustomJwtValidationMiddleware(RequestDelegate next, ILogger<CustomJwtValidationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, CustomJwtSecurityTokenHandler tokenHandler)
        {
            var token = ExtractTokenFromRequest(context.Request);
            
            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    var principal = await tokenHandler.ValidateTokenAsync(token);
                    context.User = principal;
                    _logger.LogInformation("JWT token validated successfully for user: {User}", 
                        principal.Identity?.Name ?? "Unknown");
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "JWT token validation failed");
                    // No establecer el usuario, permitir que continúe sin autenticación
                    // El atributo [Authorize] manejará la respuesta 401 si es necesario
                }
            }

            await _next(context);
        }

        private string ExtractTokenFromRequest(HttpRequest request)
        {
            var authHeader = request.Headers["Authorization"].FirstOrDefault();
            
            if (authHeader != null && authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                return authHeader.Substring("Bearer ".Length).Trim();
            }

            return null;
        }
    }
}