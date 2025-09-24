using System.Security.Claims;

namespace Rokys.Audit.WebAPI.Middleware
{
    public class IdentityServerAuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<IdentityServerAuthorizationMiddleware> _logger;

        public IdentityServerAuthorizationMiddleware(RequestDelegate next, ILogger<IdentityServerAuthorizationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip authorization for health checks and swagger endpoints
            if (context.Request.Path.StartsWithSegments("/health") ||
                context.Request.Path.StartsWithSegments("/swagger") ||
                context.Request.Path.StartsWithSegments("/.well-known"))
            {
                await _next(context);
                return;
            }

            // If user is authenticated via IdentityServer, enrich claims
            if (context.User.Identity?.IsAuthenticated == true)
            {
                await EnrichUserClaims(context);
            }

            await _next(context);
        }

        private Task EnrichUserClaims(HttpContext context)
        {
            try
            {
                var identity = context.User.Identity as ClaimsIdentity;

                if (identity != null)
                {
                    // Log the claims for debugging
                    _logger.LogInformation("User authenticated with claims:");
                    foreach (var claim in context.User.Claims)
                    {
                        _logger.LogInformation($"Claim: {claim.Type} = {claim.Value}");
                    }

                    // Extract user information from IdentityServer claims
                    var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                                context.User.FindFirst("sub")?.Value;

                    var userName = context.User.FindFirst(ClaimTypes.Name)?.Value ??
                                  context.User.FindFirst("preferred_username")?.Value;

                    var email = context.User.FindFirst(ClaimTypes.Email)?.Value ??
                               context.User.FindFirst("email")?.Value;

                    // Add custom claims if they don't exist
                    if (!string.IsNullOrEmpty(userId) && !context.User.HasClaim("user_id", userId))
                    {
                        identity.AddClaim(new Claim("user_id", userId));
                    }

                    if (!string.IsNullOrEmpty(userName) && !context.User.HasClaim("username", userName))
                    {
                        identity.AddClaim(new Claim("username", userName));
                    }

                    if (!string.IsNullOrEmpty(email) && !context.User.HasClaim("user_email", email))
                    {
                        identity.AddClaim(new Claim("user_email", email));
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error enriching user claims");
            }

            return Task.CompletedTask;
        }
    }

    public static class IdentityServerAuthorizationMiddlewareExtensions
    {
        public static IApplicationBuilder UseIdentityServerAuthorization(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<IdentityServerAuthorizationMiddleware>();
        }
    }
}