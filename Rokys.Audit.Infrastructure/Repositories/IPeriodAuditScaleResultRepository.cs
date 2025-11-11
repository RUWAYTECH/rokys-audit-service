using System.Linq.Expressions;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IPeriodAuditScaleResultRepository : IRepository<PeriodAuditScaleResult>
    {
        Task<List<PeriodAuditScaleResult>> GetByPeriodAuditGroupResultId(Guid periodAuditGroupResultId);
        Task<PeriodAuditScaleResult> GetCustomByIdAsync(Expression<Func<PeriodAuditScaleResult, bool>> filter);
        Task<bool> GetValidatorByScaleGroupIdAsync(Guid periodAuditGroupResultId, Guid scaleGroupId, Guid? excludeId = null);
    }
}
