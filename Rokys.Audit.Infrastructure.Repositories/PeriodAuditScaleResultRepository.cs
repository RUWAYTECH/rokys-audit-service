using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public class PeriodAuditScaleResultRepository : Repository<PeriodAuditScaleResult>, IPeriodAuditScaleResultRepository
    {
        public PeriodAuditScaleResultRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
