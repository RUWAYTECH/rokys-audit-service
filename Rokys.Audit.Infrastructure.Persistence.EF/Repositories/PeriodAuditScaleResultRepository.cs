using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class PeriodAuditScaleResultRepository : EFRepository<PeriodAuditScaleResult>, IPeriodAuditScaleResultRepository
    {

        public PeriodAuditScaleResultRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<List<PeriodAuditScaleResult>> GetByPeriodAuditGroupResultId(Guid periodAuditGroupResultId)
        {
            return await Db.PeriodAuditScaleResults
                .Include(x => x.ScaleGroup)
                .Where(x => x.PeriodAuditGroupResultId == periodAuditGroupResultId && x.IsActive)
                .ToListAsync();
        }

        public async Task<PeriodAuditScaleResult> GetCustomByIdAsync(Expression<Func<PeriodAuditScaleResult, bool>> filter)
        {
            var entity = await Db.PeriodAuditScaleResults
                .AsNoTracking()
                .Where(filter)
                .Include(e => e.PeriodAuditGroupResult.PeriodAudit.Store.Enterprise.ScaleCompanies)
                .Include(sg => sg.ScaleGroup)
                .Include(a => a.PeriodAuditGroupResult.PeriodAudit.PeriodAuditParticipants)
                .ThenInclude(pa => pa.UserReference)
                .Include(st => st.PeriodAuditGroupResult.PeriodAudit.AuditStatus)
                .Include(pas => pas.PeriodAuditScaleSubResults)
                .Include(pasc => pasc.PeriodAuditScoringCriteriaResults)
                .FirstOrDefaultAsync();

            return entity;
        }

        public async Task<bool> GetValidatorByScaleGroupIdAsync(Guid periodAuditGroupResultId, Guid scaleGroupId, Guid? excludeId = null)
        {
            var query = Db.PeriodAuditScaleResults
                .Where(x => x.PeriodAuditGroupResultId == periodAuditGroupResultId
                            && x.ScaleGroupId == scaleGroupId
                            && x.IsActive
                            && (excludeId == null || x.PeriodAuditScaleResultId != excludeId));

            return await query.AnyAsync();
        }

        public virtual async Task<(List<PeriodAuditScaleResult> Items, int TotalRows)> GetPagedCustomAsync(
            Expression<Func<PeriodAuditScaleResult, bool>> filter = null,
            Func<IQueryable<PeriodAuditScaleResult>, IOrderedQueryable<PeriodAuditScaleResult>> orderBy = null,
            int pageNumber = 0,
            int pageSize = 0
        )
        {
            var query = CreateDbSetQuery(filter);
            query = query
                .Include(x => x.ScaleGroup)
                .Include(x => x.PeriodAuditGroupResult)
                .Include(x => x.PeriodAuditActionPlans)
                    .ThenInclude(pa => pa.ResponsibleUser);
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
    }
}
