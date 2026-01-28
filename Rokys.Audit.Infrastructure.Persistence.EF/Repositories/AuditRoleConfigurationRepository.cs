using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using System.Linq.Expressions;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class AuditRoleConfigurationRepository : EFRepository<AuditRoleConfiguration>, IAuditRoleConfigurationRepository
    {
        public AuditRoleConfigurationRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<AuditRoleConfiguration?> GetByRoleCodeAsync(string roleCode)
        {
            if (string.IsNullOrWhiteSpace(roleCode))
                return null;

            return await DbSet
                .FirstOrDefaultAsync(x => x.RoleCode == roleCode && x.IsActive);
        }

        public async Task<bool> ExistsByRoleCodeAsync(string roleCode, Guid? enterpriseId, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(roleCode))
                return false;
            
            var query = DbSet
                .Where(x =>
                    x.RoleCode == roleCode &&
                    x.IsActive &&
                    (
                        (enterpriseId == null && x.EnterpriseId == null) ||
                        (enterpriseId != null && x.EnterpriseId == enterpriseId)
                    )
                );


            if (excludeId.HasValue)
                query = query.Where(x => x.AuditRoleConfigurationId != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<AuditRoleConfiguration>> GetActiveConfigurationsOrderedAsync()
        {
            return await DbSet
                .Include(x => x.Enterprise)
                .Where(x => x.IsActive)
                .OrderBy(x => x.SequenceOrder ?? int.MaxValue)
                .ThenBy(x => x.RoleName)
                .ToListAsync();
        }

        public async Task<bool> ExistsBySequenceOrderAsync(int sequenceOrder, Guid? enterpriseId, Guid? excludeId = null)
        {            
            var query = DbSet
                .Where(x =>
                    x.SequenceOrder == sequenceOrder &&
                    x.IsActive &&
                    (
                        (enterpriseId == null && x.EnterpriseId == null) ||
                        (enterpriseId != null && x.EnterpriseId == enterpriseId)
                    )
                );

            if (excludeId.HasValue)
                query = query.Where(x => x.AuditRoleConfigurationId != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<(List<AuditRoleConfiguration> Items, int TotalRows)> GetCustomPagedAsync(Expression<Func<AuditRoleConfiguration, bool>> filter, int pageNumber, int pageSize)
        {
            var query = Db.AuditRoleConfigurations.Where(filter)
               .Include(x => x.Enterprise)
               .Include(x => x.EnterpriseGrouping)
               .ThenInclude(eg => eg.EnterpriseGroups.Where(eg => eg.IsActive))
               .ThenInclude(e => e.Enterprise)
               .OrderByDescending(a => a.SequenceOrder)
               .ThenByDescending(a => a.RoleName);


            int rowsCount = await query.CountAsync();
            if (pageSize <= 0 || pageNumber <= 0)
            {
                var allItems = await query.ToListAsync();
                return (allItems, rowsCount);
            }
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, rowsCount);
        }
    }
}