using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class PeriodAuditRepository : EFRepository<PeriodAudit>, IPeriodAuditRepository
    {
        public PeriodAuditRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
