using Microsoft.EntityFrameworkCore;
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
        public async Task<List<PeriodAuditTableScaleTemplateResult>> GetByPeriodAuditScaleResultId(Guid periodAuditScaleResultId)
        {
            return await Db.PeriodAuditTableScaleTemplateResults
                .Where(x => x.PeriodAuditScaleResultId == periodAuditScaleResultId && x.IsActive)
                .OrderByDescending(x => x.CreationDate)
                .ToListAsync();
        }
    }
}
