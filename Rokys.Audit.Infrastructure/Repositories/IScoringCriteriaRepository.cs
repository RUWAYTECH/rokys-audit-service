using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IScoringCriteriaRepository : IRepository<ScoringCriteria>
    {
        Task<List<ScoringCriteria>> GetByScaleGroupIdAsync(Guid scaleGroupId);
    }
}
