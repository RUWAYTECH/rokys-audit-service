using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public class PeriodAuditGroupResultRepository : Repository<PeriodAuditGroupResult>, IPeriodAuditGroupResultRepository
    {
        public PeriodAuditGroupResultRepository(IDbFactory dbFactory) : base(dbFactory)
        {
        }
    }
}
