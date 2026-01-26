using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IEnterpriseGroupRepository : IRepository<EnterpriseGroup>
    {
        Task<List<EnterpriseGroup>> GetByEnterpriseGroupingId(Guid enterpriseGroupingId);
        Task<bool> ExistsEnterpriseInOtherGroupAsync(IEnumerable<Guid> enterpriseIds, Guid groupingId);
    }
}
