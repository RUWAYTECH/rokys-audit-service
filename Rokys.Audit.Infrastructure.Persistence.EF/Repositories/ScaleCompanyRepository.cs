using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class ScaleCompanyRepository : EFRepository<ScaleCompany>, IScaleCompanyRepository
    {
        private readonly ApplicationDbContext _context;
        public ScaleCompanyRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<ScaleCompany>> GetByEnterpriseIdAsync(Guid enterpriseId)
        {
            return await _context.ScaleCompanies
                .Include(enterpriseId => enterpriseId.Enterprise)
                .Where(sc => sc.EnterpriseId == enterpriseId && sc.IsActive)
                .ToListAsync();
        }
    }
}
