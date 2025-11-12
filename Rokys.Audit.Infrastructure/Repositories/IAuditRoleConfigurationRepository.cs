using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IAuditRoleConfigurationRepository : IRepository<AuditRoleConfiguration>
    {
        Task<AuditRoleConfiguration?> GetByRoleCodeAsync(string roleCode);
        Task<bool> ExistsByRoleCodeAsync(string roleCode, Guid? excludeId = null);
        Task<IEnumerable<AuditRoleConfiguration>> GetActiveConfigurationsOrderedAsync();
        Task<bool> ExistsBySequenceOrderAsync(int sequenceOrder, Guid? excludeId = null);
    }
}