using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class ScaleGroupRepository : EFRepository<ScaleGroup>, IScaleGroupRepository
    {
        private readonly ApplicationDbContext _context;

        public ScaleGroupRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _context.ScaleGroups
                .AnyAsync(x => x.Code == code && x.IsActive);
        }

        public async Task<bool> ExistsByCodeAsync(string code, Guid? excludeId)
        {
            return await _context.ScaleGroups
                .AnyAsync(x => x.Code == code && x.ScaleGroupId != excludeId && x.IsActive);
        }

        public async Task<List<ScaleGroup>> GetByGroupIdAsync(Guid groupId)
        {
            return await _context.ScaleGroups
                .Where(x => x.GroupId == groupId && x.IsActive)
                .ToListAsync();
        }
    }
}