using System.Linq.Expressions;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IPeriodAuditRepository : IRepository<PeriodAudit>
    {
        Task<PeriodAudit> GetCustomByIdAsync(Expression<Func<PeriodAudit, bool>> filter);
        Task<List<PeriodAudit>> GetCustomSearchAsync(Expression<Func<PeriodAudit, bool>> filter);
        Task<(List<PeriodAudit> Items, int TotalRows)>  GetSearchPagedAsync(Expression<Func<PeriodAudit, bool>> filter, int pageNumber, int pageSize);
        Task<List<PeriodAudit>> GetWithScaleGroup(Expression<Func<PeriodAudit, bool>>? filter = null);
    }
}
