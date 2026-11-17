using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;
using System.Linq.Expressions;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IPeriodAuditPreEvaluationRepository : IRepository<PeriodAuditPreEvaluation>
    {
        Task<List<PeriodAuditPreEvaluation>> GetByPeriodAuditGroupResultIdAsync(Guid periodAuditGroupResultId);
    }
}
