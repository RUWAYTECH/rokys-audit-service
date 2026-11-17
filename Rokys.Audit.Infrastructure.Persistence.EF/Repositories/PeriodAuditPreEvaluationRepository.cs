using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using System.Linq.Expressions;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class PeriodAuditPreEvaluationRepository : EFRepository<PeriodAuditPreEvaluation>, IPeriodAuditPreEvaluationRepository
    {
        public PeriodAuditPreEvaluationRepository(ApplicationDbContext context) : base(context)
        {
        }


        public async Task<List<PeriodAuditPreEvaluation>> GetByPeriodAuditGroupResultIdAsync(Guid periodAuditGroupResultId)
        {
            return await DbSet.Where(x => x.PeriodAuditGroupResultId == periodAuditGroupResultId && x.IsActive)
                .ToListAsync();
        }
    }
}
