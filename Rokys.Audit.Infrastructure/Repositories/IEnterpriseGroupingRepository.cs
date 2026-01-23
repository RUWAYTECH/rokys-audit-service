using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;
using System.Linq.Expressions;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IEnterpriseGroupingRepository : IRepository<EnterpriseGrouping>
    {
        Task<(List<EnterpriseGrouping> Items, int TotalRows)> GetPagedCustomAsync(
           Expression<Func<EnterpriseGrouping, bool>> filter = null,
           Func<IQueryable<EnterpriseGrouping>, IOrderedQueryable<EnterpriseGrouping>> orderBy = null,
           int pageNumber = 0,
           int pageSize = 0
       );
        Task<List<EnterpriseGrouping>> GetByEnterpriseGroupingId(Guid id);
    }
}
