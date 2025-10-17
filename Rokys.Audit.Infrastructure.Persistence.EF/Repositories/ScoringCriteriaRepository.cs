using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class ScoringCriteriaRepository : EFRepository<ScoringCriteria>, IScoringCriteriaRepository
    {
        private readonly ApplicationDbContext _context;
        public ScoringCriteriaRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<ScoringCriteria>> GetByScaleGroupIdAsync(Guid scaleGroupId)
        {
            return await _context.ScoringCriteria
                .Where(x => x.ScaleGroupId == scaleGroupId && x.IsActive)
                .ToListAsync();
        }
    }
}
