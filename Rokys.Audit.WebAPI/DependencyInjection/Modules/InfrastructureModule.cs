using System.Reflection;
using Autofac;
using Microsoft.EntityFrameworkCore;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Persistence.Dp.Query;
using Rokys.Audit.Infrastructure.Persistence.EF;
using Rokys.Audit.Infrastructure.Persistence.EF.Repositories;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;

namespace Rokys.Audit.WebAPI.DependencyInjection.Modules
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
            builder.RegisterInstance(_configuration.GetSection("FileSettings").Get<FileSettings>());


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
