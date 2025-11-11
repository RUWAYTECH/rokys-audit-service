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
                .Include(x => x.PeriodAuditParticipants)
                    .ThenInclude(p => p.UserReference)
                .Include(x => x.AuditStatus)
                .FirstOrDefaultAsync();

            return entity!;
        }

        public async Task<List<PeriodAudit>> GetCustomSearchAsync(Expression<Func<PeriodAudit, bool>> filter)
        {
            var entities = await Db.PeriodAudits.Where(filter)
                .Include(x => x.Store)
                    .ThenInclude(s => s.Enterprise)
                .Include(x => x.PeriodAuditParticipants)
                    .ThenInclude(p => p.UserReference)
                .Include(x => x.AuditStatus)
                .OrderByDescending(a=>a.CreationDate)
                .ToListAsync();

                return entities;
        }

    }
}
