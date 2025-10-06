using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class CriteriaSubResultRepository : EFRepository<CriteriaSubResult>, IRepository<CriteriaSubResult>
    {
        public CriteriaSubResultRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
