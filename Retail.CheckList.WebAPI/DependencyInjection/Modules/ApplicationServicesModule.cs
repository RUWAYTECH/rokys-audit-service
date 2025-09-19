using System.Reflection;
using Autofac;
using Retail.CheckList.Services.Services;

namespace Retail.CheckList.WebAPI.DependencyInjection.Modules
{
    public class ApplicationServicesModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(ProveedorService).GetTypeInfo().Assembly).AsImplementedInterfaces();          
        }
    }
}
