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

        public async Task<List<ScaleCompany>> GetConfiguredForEnterprise(
            Guid? enterpriseGroupingId,
            Guid enterpriseId
        )
        {
            // If: there are ScaleCompanies linked to the Enterprise, return them
            var byEnterprise = await _context.ScaleCompanies
                .Include(enterpriseId => enterpriseId.Enterprise)
                .Where(sc => sc.EnterpriseId == enterpriseId && sc.IsActive)
                .ToListAsync();
            if (byEnterprise != null && byEnterprise.Count > 0)
            {
                return byEnterprise;
            }

            // If: there are ScaleCompanies linked to the Enterprise's Grouping, return them
            if (enterpriseGroupingId.HasValue)
            {
                var byGroup = await _context.ScaleCompanies
                .Include(sc => sc.Enterprise)
                .Where(sc => sc.EnterpriseGroupingId == enterpriseGroupingId && sc.IsActive)
                .ToListAsync();

                if (byGroup != null && byGroup.Count > 0)
                {
                    return byGroup;
                }
            }

            // Else: return the default ScaleCompanies (not linked to any Enterprise or Grouping)
            return await GetAsync(filter: e => e.EnterpriseId == null && e.EnterpriseGroupingId == null);
        }
    }
}
