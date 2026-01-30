using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;
using System.Linq.Expressions;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IAuditRoleConfigurationRepository : IRepository<AuditRoleConfiguration>
    {
        Task<AuditRoleConfiguration?> GetByRoleCodeAsync(string roleCode);
        Task<bool> ExistsByRoleCodeAsync(string roleCode, Guid? enterpriseId, Guid? excludeId = null);
        Task<IEnumerable<AuditRoleConfiguration>> GetActiveConfigurationsOrderedAsync();
        Task<bool> ExistsBySequenceOrderAsync(int sequenceOrder, Guid? enterpriseId, Guid? excludeId = null);
        Task<(List<AuditRoleConfiguration> Items, int TotalRows)> GetCustomPagedAsync(Expression<Func<AuditRoleConfiguration, bool>> filter, int pageNumber, int pageSize);
        Task<List<AuditRoleConfiguration>> GetByEnterpriseId(Guid? enterpriseId);
    }
}