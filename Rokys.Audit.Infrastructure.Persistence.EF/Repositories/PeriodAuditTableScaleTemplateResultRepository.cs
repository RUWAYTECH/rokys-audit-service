using System.Linq.Expressions;
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

        public async Task<List<PeriodAuditTableScaleTemplateResult>> GetForReport(Expression<Func<PeriodAuditTableScaleTemplateResult, bool>>? filter)
        {
            var query = Db.PeriodAuditTableScaleTemplateResults.Where(filter ?? (_ => true))
                .Include(x => x.PeriodAuditFieldValues)
                .Include(x => x.PeriodAuditScaleResult)
                    .ThenInclude(x => x.ScaleGroup)
                        .ThenInclude(sg => sg!.Group)
                .Include(x => x.PeriodAuditScaleResult)
                    .ThenInclude(x => x.PeriodAuditGroupResult)
                        .ThenInclude(x => x.PeriodAudit)
                            .ThenInclude(x => x.Store)
                                .ThenInclude(s => s!.Enterprise)
                .Include(x => x.PeriodAuditScaleResult)
                    .ThenInclude(x => x.PeriodAuditGroupResult)
                        .ThenInclude(x => x.PeriodAudit)
                            .ThenInclude(x => x.AuditStatus)
                .Include(x => x.PeriodAuditScaleResult)
                    .ThenInclude(x => x.PeriodAuditGroupResult)
                        .ThenInclude(x => x.PeriodAudit)
                            .ThenInclude(x => x.PeriodAuditGroupResults);

            return await query.ToListAsync();
        }
    }
}
