using Microsoft.EntityFrameworkCore;
using Rokys.Audit.Infrastructure.Persistence.EF.Storage;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Persistence.EF.Repositories
{
    public class EmployeeStoreRepository : EFRepository<EmployeeStore>, IEmployeeStoreRepository
    {
        public EmployeeStoreRepository(ApplicationDbContext context) : base(context)
        {
        }
        public async Task<List<EmployeeStore>> GetByUserReferenceIdAsync(Guid userReferenceId)
        {
            return await DbSet
                .Where(es => es.UserReferenceId == userReferenceId)
                .ToListAsync();
        }
    }
}
