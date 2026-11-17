using System.Linq.Expressions;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IScaleCompanyRepository : IRepository<ScaleCompany>
    {
        Task<List<ScaleCompany>> GetByEnterpriseIdAsync(Guid enterpriseId);
        Task<(List<ScaleCompany> Items, int TotalRows)>  GetCustomPagedAsync(Expression<Func<ScaleCompany, bool>> filter, int pageNumber, int pageSize);
        Task<List<ScaleCompany>> GetConfiguredForEnterprise(
            Guid? enterpriseGroupingId,
            Guid enterpriseId
        );
    }
}
