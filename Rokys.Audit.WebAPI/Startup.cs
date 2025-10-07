using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Rokys.Audit.Infrastructure.Mapping.AM;
using Rokys.Audit.Infrastructure.Persistence.Dp;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Subscription.Hub.Extensions;
using Rokys.Audit.WebAPI.Configuration;
using Rokys.Audit.WebAPI.DependencyInjection;
using Rokys.Audit.WebAPI.Filters;
using Rokys.Audit.WebAPI.Middleware;
using System.Data;

namespace Rokys.Audit.WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public IServiceCollection Services { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            Services = services;
            Services.AddMvc();
            Services.AddCors();
            Services.AddControllers();
            Services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(option =>
            {
                //c.SwaggerDoc("v1", new OpenApiInfo { Title = "swaggertest", Version = "v1" });
                option.SchemaFilter<CustomSwaggerSchemaFilter>();
                option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter a valid token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                option.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type=ReferenceType.SecurityScheme,
                                Id="Bearer"
                            }
                        },
                        new string[]{}
                    }
                });
            });
            //AddAutoMapper - to load all automapper profiles
            services.AddAutoMapper(typeof(AMMapper));
            var connectionString = Configuration.GetConnectionString("Main");
            //EF
            services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(connectionString));

            //Dapper
            services.AddTransient<IDbConnection>((sp) =>
            {
                return new SqlConnection(connectionString);
            });

            // Add HttpClient factory
            services.AddHttpClient();

            // Add IdentityServer service
            services.AddHttpClient<Services.IIdentityServerService, Services.IdentityServerService>();
            services.AddScoped<Services.IIdentityServerService, Services.IdentityServerService>();

            ContextDp.Config();
            this.AddSecurity();
            
            // Add Subscription Hub
            Services.AddSubscriptionHub(Configuration);
            
            return DependencyConfig.Configure(Services, Configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, Microsoft.AspNetCore.Hosting.IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            string[] domains = Configuration.GetSection("AllowedHosts").Value.Split(";");

            app.UseCors(x => x.WithOrigins(domains)
                .AllowAnyMethod()
                .AllowAnyHeader()
                );

            //Logger file
            loggerFactory.AddFile(env.ContentRootPath + "/LogError/log-{Date}.txt");

            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseMiddleware<CustomJwtValidationMiddleware>(); // Add custom JWT validation middleware
            app.UseIdentityServerAuthorization(); // Add custom middleware after authentication
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
