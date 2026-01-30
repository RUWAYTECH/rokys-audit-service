using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using System.Linq.Expressions;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class EnterpriseGroupingRepository : EFRepository<EnterpriseGrouping>, IEnterpriseGroupingRepository
    {
        public EnterpriseGroupingRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<List<EnterpriseGrouping>> GetByEnterpriseGroupingId(Guid id)
        {
            return await Db.EnterpriseGroupings
                .Include(x => x.EnterpriseGroups)
                .ThenInclude(pa => pa.Enterprise)
                .Where(x => x.EnterpriseGroupingId == id && x.IsActive)
                .ToListAsync();
        }
        public async Task<(List<EnterpriseGrouping> Items, int TotalRows)> GetPagedCustomAsync(
            Expression<Func<EnterpriseGrouping, bool>> filter = null,
            Func<IQueryable<EnterpriseGrouping>,
            IOrderedQueryable<EnterpriseGrouping>> 
            orderBy = null, int pageNumber = 0, int pageSize = 0)
        {
            var query = CreateDbSetQuery(filter);
            query = query
                .Include(x => x.EnterpriseGroups.Where(eg => eg.IsActive))
                    .ThenInclude(pa => pa.Enterprise);
            if (orderBy != null)
                query = orderBy(query);

            int rowsCount = await query.CountAsync();
            if (pageSize <= 0 || pageNumber <= 0)
            {
                var allItems = await query.ToListAsync();
                return (allItems, rowsCount);
            }
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return (items, rowsCount);
        }
        public async Task<EnterpriseGrouping> GetFirstByEnterpriseGroupingId(Guid id)
        {
            return await Db.EnterpriseGroupings
                .Include(x => x.EnterpriseGroups.Where(eg => eg.IsActive))
                .ThenInclude(pa => pa.Enterprise)
                .Where(x => x.EnterpriseGroupingId == id && x.IsActive)
                .FirstOrDefaultAsync();
        }

        public async Task<EnterpriseGrouping> GetFirstEnterpriseGroupingByEnterpriseId(Guid enterpriseId)
        {
            return await Db.EnterpriseGroupings
                .Include(x => x.EnterpriseGroups.Where(eg => eg.IsActive))
                .ThenInclude(pa => pa.Enterprise)
                .Where(x => x.EnterpriseGroups.Any(eg => eg.EnterpriseId == enterpriseId && eg.IsActive) && x.IsActive)
                .FirstOrDefaultAsync();
        }
    }
}
