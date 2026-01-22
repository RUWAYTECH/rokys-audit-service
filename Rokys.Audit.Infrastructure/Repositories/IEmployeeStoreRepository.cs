using Rokys.Audit.DTOs.Responses.EmployeeStore;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IEmployeeStoreRepository : IRepository<EmployeeStore>
    {
        Task<List<EmployeeStore>> GetByUserReferenceIdAsync(Guid UserReferenceId);
    }
}
