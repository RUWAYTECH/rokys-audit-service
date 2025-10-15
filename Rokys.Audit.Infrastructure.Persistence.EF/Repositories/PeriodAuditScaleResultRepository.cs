using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class PeriodAuditScaleResultRepository : EFRepository<PeriodAuditScaleResult>, IPeriodAuditScaleResultRepository
    {
        private readonly ApplicationDbContext _context;
        public PeriodAuditScaleResultRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<PeriodAuditScaleResult>> GetByPeriodAuditGroupResultId(Guid periodAuditGroupResultId)
        {
            return await _context.PeriodAuditScaleResults
                .Include(x => x.ScaleGroup)
                .Where(x => x.PeriodAuditGroupResultId == periodAuditGroupResultId && x.IsActive)
                .ToListAsync();
        }
    }
}
