using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class SubScaleRepository : EFRepository<SubScale>, ISubScaleRepository
    {
        private readonly ApplicationDbContext _context;
        public SubScaleRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<SubScale>> GetByEnterpriseGroupingIdAsync(Guid enterpriseGroupingId)
        {
            return await _context.SubScales
                .Include(s => s.EnterpriseGrouping)
                .Where(s => s.EnterpriseGroupingId == enterpriseGroupingId && s.IsActive)
                .ToListAsync();
        }
    }
}
