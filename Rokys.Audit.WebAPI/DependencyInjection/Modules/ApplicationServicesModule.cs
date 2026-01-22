using System.Reflection;
using Autofac;
using Rokys.Audit.External.Services;
using Rokys.Audit.Services.Services;

namespace Rokys.Audit.WebAPI.DependencyInjection.Modules
{
    public class ApplicationServicesModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(ScaleCompanyService).GetTypeInfo().Assembly).AsImplementedInterfaces();
            builder.RegisterAssemblyTypes(typeof(EmailService).GetTypeInfo().Assembly).AsImplementedInterfaces();
        }
    }
}
