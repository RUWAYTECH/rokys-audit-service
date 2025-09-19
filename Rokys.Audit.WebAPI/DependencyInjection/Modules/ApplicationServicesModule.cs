using System.Reflection;
using Autofac;
using Rokys.Audit.Services.Services;

namespace Rokys.Audit.WebAPI.DependencyInjection.Modules
{
    public class ApplicationServicesModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(ProveedorService).GetTypeInfo().Assembly).AsImplementedInterfaces();          
        }
    }
}
