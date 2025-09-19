using System.Reflection;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Retail.CheckList.DTOs.Common;
using Retail.CheckList.Infrastructure.Persistence.Abstract;
using Retail.CheckList.Infrastructure.Persistence.Dp.Query;
using Retail.CheckList.Infrastructure.Persistence.EF;
using Retail.CheckList.Infrastructure.Persistence.EF.Repositories;
using Retail.CheckList.Infrastructure.Persistence.EF.Storage;

namespace Retail.CheckList.WebAPI.DependencyInjection.Modules
{
    public class InfrastructureModule : Autofac.Module
    {
        private readonly IConfiguration _configuration;

        public InfrastructureModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        protected override void Load(ContainerBuilder builder)
        {

            builder.RegisterInstance(_configuration.GetSection("JwtSettings").Get<JwtSettings>());
            

            builder.RegisterAssemblyTypes(typeof(ProveedorRepository).GetTypeInfo().Assembly).AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(ProveedorDp).GetTypeInfo().Assembly).AsImplementedInterfaces();

            //Register Command repositories
            builder.RegisterGeneric(typeof(EFRepository<>)).As(typeof(IRepository<>)).InstancePerRequest();

            builder.RegisterType<EFUnitOfWork>().As<IUnitOfWork>();

            //Register EF Context
            builder.RegisterType<ApplicationDbContext>().As<DbContext>().InstancePerRequest();
        }
    }
}
