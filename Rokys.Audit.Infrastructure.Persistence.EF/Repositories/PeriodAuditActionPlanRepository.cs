using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class PeriodAuditActionPlanRepository : EFRepository<PeriodAuditActionPlan>, IPeriodAuditActionPlanRepository
    {
        public PeriodAuditActionPlanRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
