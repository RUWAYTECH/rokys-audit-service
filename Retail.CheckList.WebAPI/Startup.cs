using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Retail.CheckList.Infrastructure.Mapping.AM;
using Retail.CheckList.Infrastructure.Persistence.Dp;
using Retail.CheckList.Infrastructure.Persistence.EF.Storage;
using Retail.CheckList.WebAPI.Configuration;
using Retail.CheckList.WebAPI.DependencyInjection;
using Retail.CheckList.WebAPI.Filters;
using System.Data;

namespace Retail.CheckList.WebAPI
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
            ContextDp.Config();
            this.AddSecurity();
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
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
