using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class SystemConfigurationRepository : EFRepository<SystemConfiguration>, ISystemConfigurationRepository
    {
        public SystemConfigurationRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
