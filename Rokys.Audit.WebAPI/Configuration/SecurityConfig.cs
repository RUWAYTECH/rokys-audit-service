using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Rokys.Audit.WebAPI.Services;

namespace Rokys.Audit.WebAPI.Configuration
{
    public static class SecurityConfig
    {

        public static Startup AddSecurity(this Startup startup)
        {
            var section = startup.Configuration.GetSection("Security");

            if (Convert.ToBoolean(section["Enabled"]))
            {
                // Registrar servicios personalizados para JWT
                startup.Services.AddHttpClient<CustomJwtSecurityTokenHandler>();
                startup.Services.AddScoped<CustomJwtSecurityTokenHandler>();

                startup.Services.AddAuthorization();

                startup.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = Convert.ToBoolean(startup.Configuration["IdentityServer:RequireHttpsMetadata"]);
                    cfg.SaveToken = true;

                    // Configuración básica - usaremos nuestro middleware personalizado para la validación real
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false, // Nuestro middleware lo validará
                        ValidateAudience = false,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = false,
                        RequireSignedTokens = false
                    };

                    cfg.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                            logger.LogDebug("JWT token received from request");
                            return Task.CompletedTask;
                        },
                        OnAuthenticationFailed = context =>
                        {
                            var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<Startup>>();
                            logger.LogDebug("Default JWT authentication failed, will use custom middleware");
                            // No marcamos como fallido, nuestro middleware se encargará
                            context.NoResult();
                            return Task.CompletedTask;
                        }
                    };
                });

            }

            return startup;

        }

    }
}
