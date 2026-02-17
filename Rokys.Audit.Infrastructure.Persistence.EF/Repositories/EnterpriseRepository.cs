using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using System.Linq.Expressions;

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

        public async Task<(List<Enterprise> Items, int TotalRows)> GetCustomPagedAsync(Expression<Func<Enterprise, bool>> filter, int pageNumber, int pageSize)
        {
            var query = Db.Enterprises.Where(filter)
                .Include(x => x.EnterpriseGroups.Where(eg => eg.IsActive))
                    .ThenInclude(x => x.EnterpriseGrouping)
                .Include(x => x.Theme)
               .OrderByDescending(a => a.Name);


            int rowsCount = await query.CountAsync();
            if (pageSize <= 0 || pageNumber <= 0)
            {
                var allItems = await query.ToListAsync();
                return (allItems, rowsCount);
            }
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, rowsCount);
        }
    }
}
