using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class PeriodAuditGroupResultRepository : EFRepository<PeriodAuditGroupResult>, IPeriodAuditGroupResultRepository
    {
        private readonly ApplicationDbContext _context;
        public PeriodAuditGroupResultRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<PeriodAuditGroupResult>> GetByPeriodAuditIdAsync(Guid periodAuditId, Guid? id = null)
        {
            return await _context.PeriodAuditGroupResults
                .Where(pagr => pagr.PeriodAuditId == periodAuditId && pagr.IsActive && (id == null || pagr.PeriodAuditGroupResultId != id))
                .ToListAsync();
        }

        public async Task<List<PeriodAuditGroupResult>> GetByPeriodAuditIdWithScaleResultsAsync(Guid periodAuditId)
        {
            return await _context.PeriodAuditGroupResults
                .Include(pagr => pagr.PeriodAuditScaleResults)
                    .ThenInclude(pasr => pasr.ScaleGroup)
                .Where(pagr => pagr.PeriodAuditId == periodAuditId && pagr.IsActive)
                .OrderBy(pagr => pagr.SortOrder)
                .ToListAsync();
        }

        public async Task<bool> GetValidatorByGroupIdAsync(Guid periodAuditId, Guid groupId, Guid? id = null)
        {
            return await _context.PeriodAuditGroupResults
                .AsNoTracking()
                .AnyAsync(pagr =>
                    pagr.PeriodAuditId == periodAuditId &&
                    pagr.GroupId == groupId &&
                    pagr.IsActive &&
                    (id == null || pagr.PeriodAuditGroupResultId != id)
                );
        }
    }
}
