using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;
using System.Collections.Generic;

namespace Rokys.Audit.Infrastructure.Repositories
{
    public interface IEnterpriseRepository : IRepository<Enterprise>
    {
        Task<Enterprise?> GetByEnterpriseId(Guid EnterpriseId);
    }
}
