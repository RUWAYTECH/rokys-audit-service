using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class EnterpriseGroupRepository : EFRepository<EnterpriseGroup>, IEnterpriseGroupRepository
    {
        public EnterpriseGroupRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<List<EnterpriseGroup>> GetByEnterpriseGroupingId(Guid enterpriseGroupingId)
        {
            return await DbSet
                .Where(eg => eg.EnterpriseGroupingId == enterpriseGroupingId)
                .ToListAsync();
        }
        public async Task<bool> ExistsEnterpriseInOtherGroupAsync(
            IEnumerable<Guid> enterpriseIds,
            Guid groupingId)
        {
            return await DbSet
                .AnyAsync(eg =>
                    enterpriseIds.Contains(eg.EnterpriseId) &&
                    eg.IsActive &&
                    eg.EnterpriseGroupingId != groupingId);
        }
    }
}
