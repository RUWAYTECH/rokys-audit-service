using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface ICriteriaSubResultRepository : IRepository<CriteriaSubResult>
    {
        Task<List<CriteriaSubResult>> GetByScaleGroupIdAsync(Guid scaleGroupId);
    }
}
