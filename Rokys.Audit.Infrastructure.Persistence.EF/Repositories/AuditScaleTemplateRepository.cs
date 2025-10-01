using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class AuditScaleTemplateRepository : EFRepository<AuditScaleTemplate>, IAuditScaleTemplateRepository
    {
        public AuditScaleTemplateRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> ExistsByCodeAsync(string code, Guid? excludeId = null)
        {
            var query = DbSet.Where(x => x.Code == code && x.IsActive);
            
            if (excludeId.HasValue)
            {
                query = query.Where(x => x.AuditScaleTemplateId != excludeId.Value);
            }

            return await query.AnyAsync();
        }
    }
}