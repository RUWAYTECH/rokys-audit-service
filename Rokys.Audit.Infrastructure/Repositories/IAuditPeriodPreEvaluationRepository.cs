using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;
using System.Linq.Expressions;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IAuditPeriodPreEvaluationRepository : IRepository<AuditPeriodPreEvaluation>
    {
        Task<List<AuditPeriodPreEvaluation>> GetByPeriodAuditGroupResultIdAsync(Guid periodAuditGroupResultId);
    }
}
