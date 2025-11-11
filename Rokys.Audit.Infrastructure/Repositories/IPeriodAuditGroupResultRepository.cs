using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IPeriodAuditGroupResultRepository : IRepository<PeriodAuditGroupResult>
    {
        Task<List<PeriodAuditGroupResult>> GetByPeriodAuditIdAsync(Guid periodAuditId, Guid? id = null);
        Task<bool> GetValidatorByGroupIdAsync(Guid periodAuditId, Guid groupId, Guid? id = null);
    }
}
