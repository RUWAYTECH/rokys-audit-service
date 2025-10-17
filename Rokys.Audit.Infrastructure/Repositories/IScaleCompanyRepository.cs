using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IScaleCompanyRepository : IRepository<ScaleCompany>
    {
        Task<List<ScaleCompany>> GetByEnterpriseIdAsync(Guid enterpriseId);
    }
}
