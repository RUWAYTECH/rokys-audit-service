using Rokys.Audit.Infrastructure.Persistence.EF;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class PeriodAuditTableScaleTemplateResultRepository : EFRepository<PeriodAuditTableScaleTemplateResult>, IPeriodAuditTableScaleTemplateResultRepository
    {
        public PeriodAuditTableScaleTemplateResultRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
