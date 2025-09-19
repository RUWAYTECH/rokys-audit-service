using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Retail.CheckList.WebAPI.Configuration
{
    public static class SecurityConfig
    {

        public static Startup AddSecurity(this Startup startup)
        {
            var section = startup.Configuration.GetSection("Security");

            if (Convert.ToBoolean(section["Enabled"]))
            {
                startup.Services.AddAuthorization();

                startup.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(cfg =>
                {
                    cfg.RequireHttpsMetadata = false;
                    cfg.SaveToken = true;
                    cfg.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = startup.Configuration["JwtSettings:Issuer"],
                        ValidAudience = startup.Configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(startup.Configuration["JwtSettings:Key"])),
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = true
                    };
                });
               
            }

            return startup;

        }

    }
}
