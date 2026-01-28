using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class GroupRepository : EFRepository<Group>, IGroupRepository
    {
        private readonly ApplicationDbContext _context;
        public GroupRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<Group>> GetConfiguredForEnterprise(
            Guid? enterpriseGroupingId,
            Guid enterpriseId
        )
        {
            // If: there are Groups linked to the Enterprise, return them
            var byEnterprise = await _context.Groups
                .Where(sc => sc.EnterpriseId == enterpriseId && sc.IsActive)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();
            if (byEnterprise != null && byEnterprise.Count > 0)
            {
                return byEnterprise;
            }

            // If: there are Groups linked to the Enterprise's Grouping, return them
            if (enterpriseGroupingId.HasValue)
            {
                var byGroup = await _context.Groups
                .Where(sc => sc.EnterpriseGroupingId == enterpriseGroupingId && sc.IsActive)
                .OrderBy(x => x.SortOrder)
                .ToListAsync();

                if (byGroup != null && byGroup.Count > 0)
                {
                    return byGroup;
                }
            }

            // Else: return the default Groups (not linked to any Enterprise or Grouping)
            return await GetAsync(filter: e => e.EnterpriseId == null && e.EnterpriseGroupingId == null && e.IsActive, orderBy: q => q.OrderBy(x => x.SortOrder));
        }
    }
}