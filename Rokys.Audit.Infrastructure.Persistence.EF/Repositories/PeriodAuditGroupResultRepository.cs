using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using System.Linq.Expressions;

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

        public async Task<List<PeriodAuditGroupResult>> GetByPeriodAuditIdWithScaleResultsAsync(Expression<Func<PeriodAuditGroupResult, bool>>? filter = null)
        {
            IQueryable<PeriodAuditGroupResult> query = _context.PeriodAuditGroupResults
                .Where(filter ?? (pagr => pagr.IsActive)) // Si no se proporciona un filtro, se aplica uno que solo incluye resultados activos
                .Include(pagr => pagr.Group)
                .Include(pagr => pagr.PeriodAudit)
                .Include(pagr => pagr.PeriodAuditScaleResults)
                    .ThenInclude(pasr => pasr.ScaleGroup);
            return await query
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
