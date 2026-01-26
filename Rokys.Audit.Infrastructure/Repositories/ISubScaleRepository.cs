using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface ISubScaleRepository : IRepository<SubScale>
    {
        Task<List<SubScale>> GetByScaleCompanyIdAsync(Guid scaleCompanyId);
    }
}
