using System.Linq.Expressions;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IPeriodAuditTableScaleTemplateResultRepository : IRepository<PeriodAuditTableScaleTemplateResult>
    {
        Task<List<PeriodAuditTableScaleTemplateResult>> GetByPeriodAuditScaleResultId(Guid periodAuditScaleResultId);
        Task<List<PeriodAuditTableScaleTemplateResult>> GetForReport(Expression<Func<PeriodAuditTableScaleTemplateResult, bool>>? filter);
    }
}
