using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class AuditTemplateFieldRepository : EFRepository<AuditTemplateFields>, IAuditTemplateFieldRepository
    {
        private readonly ApplicationDbContext _context;
        public AuditTemplateFieldRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _context.AuditTemplateFields
                .AnyAsync(x => x.FieldCode == code && x.IsActive);
        }
        public async Task<bool> ExistsByCodeAsync(string code, Guid? excludeId)
        {
            return await _context.AuditTemplateFields
                .AnyAsync(x => x.FieldCode == code && x.AuditTemplateFieldId != excludeId && x.IsActive);
        }
    }
}
