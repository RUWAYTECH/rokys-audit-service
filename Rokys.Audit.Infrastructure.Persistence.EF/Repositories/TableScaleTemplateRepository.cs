using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class TableScaleTemplateRepository : EFRepository<TableScaleTemplate>, ITableScaleTemplateRepository
    {
        private readonly ApplicationDbContext _context;

        public TableScaleTemplateRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<bool> ExistsByCodeAsync(string code)
        {
            return await _context.TableScaleTemplates
                .AnyAsync(x => x.Code == code && x.IsActive);
        }

        public async Task<bool> ExistsByCodeAsync(string code, Guid? excludeId)
        {
            return await _context.TableScaleTemplates
                .AnyAsync(x => x.Code == code && x.TableScaleTemplateId != excludeId && x.IsActive);
        }

        public async Task<IEnumerable<TableScaleTemplate>> GetByScaleGroupIdAsync(Guid scaleGroupId)
        {
            return await _context.TableScaleTemplates
                .Where(x => x.ScaleGroupId == scaleGroupId && x.IsActive)
                .Include(x => x.ScaleGroup)
                .OrderBy(x => x.Name)
                .ToListAsync();
        }
    }
}