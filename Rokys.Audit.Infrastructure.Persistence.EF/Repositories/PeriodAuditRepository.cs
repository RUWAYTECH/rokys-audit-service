using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class PeriodAuditRepository : EFRepository<PeriodAudit>, IPeriodAuditRepository
    {
        public PeriodAuditRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<PeriodAudit> GetCustomByIdAsync(Expression<Func<PeriodAudit, bool>> filter)
        {
            var entity = await Db.PeriodAudits.Where(filter)
          .Include(x => x.Store)
        .ThenInclude(s => s.Enterprise)
          .Include(x => x.AuditStatus)
          .FirstOrDefaultAsync();

            if (entity != null)
            {
                // ✅ Cargar participantes ordenados por separado
                await Db.Entry(entity)
                    .Collection(x => x.PeriodAuditParticipants)
                    .Query()
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.RoleCodeSnapshot) // ✅ Orden por código de rol
                    .Include(p => p.UserReference)
                    .LoadAsync();
            }

            return entity!;
        }

        public async Task<List<PeriodAudit>> GetCustomSearchAsync(Expression<Func<PeriodAudit, bool>> filter)
        {
            var entities = await Db.PeriodAudits.Where(filter)
                .Include(x => x.Store)
                .ThenInclude(s => s.Enterprise)
                .Include(x => x.AuditStatus)
                .OrderByDescending(a => a.CreationDate)
                .ToListAsync();

            // ✅ Cargar participantes ordenados para cada entidad
            foreach (var entity in entities)
            {
                await Db.Entry(entity)
                    .Collection(x => x.PeriodAuditParticipants)
                    .Query()
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.RoleCodeSnapshot)
                    .Include(p => p.UserReference)
                    .LoadAsync();
            }

            return entities;
        }

        public async Task<(List<PeriodAudit> Items, int TotalRows)> GetSearchPagedAsync(Expression<Func<PeriodAudit, bool>> filter, int pageNumber, int pageSize)
        {
            var query = Db.PeriodAudits.Where(filter)
               .Include(x => x.Store)
               .ThenInclude(s => s.Enterprise)
               .Include(x => x.AuditStatus)
               .Include(x => x.PeriodAuditParticipants)
               .ThenInclude(p => p.UserReference)
               .OrderByDescending(a=>a.CreationDate);
                

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
