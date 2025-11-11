using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

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

        public async Task<bool> ExistsByRoleCodeAsync(string roleCode, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(roleCode))
                return false;

            var query = DbSet
                .Where(x => x.RoleCode == roleCode && x.IsActive);
            
            if (excludeId.HasValue)
                query = query.Where(x => x.AuditRoleConfigurationId != excludeId.Value);

            return await query.AnyAsync();
        }

        public async Task<IEnumerable<AuditRoleConfiguration>> GetActiveConfigurationsOrderedAsync()
        {
            return await DbSet
                .Where(x => x.IsActive)
                .OrderBy(x => x.SequenceOrder ?? int.MaxValue)
                .ThenBy(x => x.RoleName)
                .ToListAsync();
        }
    }
}