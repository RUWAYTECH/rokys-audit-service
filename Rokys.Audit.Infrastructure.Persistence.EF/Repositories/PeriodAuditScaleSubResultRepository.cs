using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class PeriodAuditScaleSubResultRepository : EFRepository<PeriodAuditScaleSubResult>, IPeriodAuditScaleSubResultRepository
    {
        public PeriodAuditScaleSubResultRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
