using System;
using System.Data;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FluentValidation;
using Retail.CheckList.Infrastructure.IMapping;
using Retail.CheckList.Infrastructure.Mapping.AM;
using Retail.CheckList.Services.Validations;
using Retail.CheckList.WebAPI.DependencyInjection.Modules;

namespace Retail.CheckList.WebAPI.DependencyInjection
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
