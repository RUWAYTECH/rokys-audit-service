using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class MaintenanceDetailTableRepository : EFRepository<MaintenanceDetailTable>, IRepository<MaintenanceDetailTable>
    {
        public MaintenanceDetailTableRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
