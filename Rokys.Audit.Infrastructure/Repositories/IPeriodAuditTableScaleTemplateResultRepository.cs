using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IPeriodAuditTableScaleTemplateResultRepository : IRepository<PeriodAuditTableScaleTemplateResult>
    {
        Task<List<PeriodAuditTableScaleTemplateResult>> GetByPeriodAuditScaleResultId(Guid periodAuditScaleResultId);
    }
}
