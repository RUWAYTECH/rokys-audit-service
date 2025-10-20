using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.EF;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class PeriodAuditTableScaleTemplateResultRepository : EFRepository<PeriodAuditTableScaleTemplateResult>, IPeriodAuditTableScaleTemplateResultRepository
    {
        private readonly ApplicationDbContext _context;
        public PeriodAuditTableScaleTemplateResultRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<PeriodAuditTableScaleTemplateResult>> GetByPeriodAuditScaleResultId(Guid periodAuditScaleResultId)
        {
            return await _context.PeriodAuditTableScaleTemplateResults
                .Where(x => x.PeriodAuditScaleResultId == periodAuditScaleResultId && x.IsActive)
                .OrderByDescending(x => x.CreationDate)
                .ToListAsync();
        }
    }
}
