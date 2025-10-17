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
        public async Task<List<PeriodAuditGroupResult>> GetByPeriodAuditIdAsync(Guid periodAuditId)
        {
            return await _context.PeriodAuditGroupResults
                .Where(pagr => pagr.PeriodAuditId == periodAuditId && pagr.IsActive)
                .ToListAsync();
        }
    }
}
