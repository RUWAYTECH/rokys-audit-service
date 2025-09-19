using System;
using System.Data;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Mapping.AM;
using Rokys.Audit.Services.Validations;
using Rokys.Audit.WebAPI.DependencyInjection.Modules;

namespace Rokys.Audit.WebAPI.DependencyInjection
{
    public class DependencyConfig
    {
        public static IServiceProvider Configure(IServiceCollection services, IConfiguration configuration)
        {
            services.AddValidatorsFromAssemblyContaining<ProveedorValidator>(ServiceLifetime.Transient);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            var builder = new ContainerBuilder();
            builder.Populate(services);
            builder.RegisterType<AMMapper>().As<IAMapper>();

            builder.RegisterModule<ApplicationServicesModule>();
            builder.RegisterModule(new InfrastructureModule(configuration));           

            var container = builder.Build();
            return new AutofacServiceProvider(container);
        }

    }
}
