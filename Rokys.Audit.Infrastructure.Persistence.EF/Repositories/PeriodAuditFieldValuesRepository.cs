using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class PeriodAuditFieldValuesRepository : EFRepository<PeriodAuditFieldValues>, IRepository<PeriodAuditFieldValues>
    {
        public PeriodAuditFieldValuesRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
