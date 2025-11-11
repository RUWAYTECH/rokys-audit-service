using System.Linq.Expressions;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IPeriodAuditRepository : IRepository<PeriodAudit>
    {
        Task<PeriodAudit> GetCustomByIdAsync(Expression<Func<PeriodAudit, bool>> filter);
        Task<List<PeriodAudit>> GetCustomSearchAsync(Expression<Func<PeriodAudit, bool>> filter);
    }
}
