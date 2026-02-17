using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IEnterpriseRepository : IRepository<Enterprise>
    {
        Task<Enterprise?> GetByEnterpriseId(Guid EnterpriseId);
        Task<(List<Enterprise> Items, int TotalRows)> GetCustomPagedAsync(Expression<Func<Enterprise, bool>> filter, int pageNumber, int pageSize);
    }
}
