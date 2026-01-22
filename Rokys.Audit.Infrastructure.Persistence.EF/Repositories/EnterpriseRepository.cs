using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class EnterpriseRepository : EFRepository<Enterprise>, IEnterpriseRepository
    {
        public EnterpriseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Enterprise?> GetByEnterpriseId(Guid enterpriseId)
        {
            var trackedEntity = Db.ChangeTracker
                .Entries<Enterprise>()
                .FirstOrDefault(e => e.Entity.EnterpriseId == enterpriseId);

            if (trackedEntity != null)
            {
                trackedEntity.State = EntityState.Detached;
            }

            return await DbSet
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EnterpriseId == enterpriseId);
        }
    }
}
