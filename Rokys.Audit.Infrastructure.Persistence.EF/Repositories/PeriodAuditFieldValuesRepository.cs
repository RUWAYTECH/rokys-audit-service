using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class PeriodAuditFieldValuesRepository : EFRepository<PeriodAuditFieldValues>, IPeriodAuditFieldValuesRepository
    {
        public PeriodAuditFieldValuesRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
