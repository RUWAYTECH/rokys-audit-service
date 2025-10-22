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
        public async Task<bool> GetValidatorByGroupIdAsync(string code, Guid groupId, Guid? excludeId)
        {
            var group = await _context.ScaleGroups
                .AsNoTracking()
                .Include(x => x.Group.Enterprise)
                .FirstOrDefaultAsync(x => x.GroupId == groupId && x.IsActive);

            if (group == null)
                return false;

            var normalizedCode = code.ToLower();

            var exists = await _context.ScaleGroups
                .AnyAsync(x => x.Group.EnterpriseId == group.Group.EnterpriseId &&
                               x.Code.ToLower() == normalizedCode &&
                               (excludeId == null || x.ScaleGroupId != excludeId) &&
                               x.IsActive);

            return exists;
        }
    }
}