using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class CriteriaSubResultRepository : EFRepository<CriteriaSubResult>, ICriteriaSubResultRepository
    {
        private readonly ApplicationDbContext _context;
        public CriteriaSubResultRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<CriteriaSubResult>> GetByScaleGroupIdAsync(Guid scaleGroupId)
        {
            return await _context.CriteriaSubResults
                .Where(x => x.ScaleGroupId == scaleGroupId && x.IsActive)
                .ToListAsync();
        }
    }
}
