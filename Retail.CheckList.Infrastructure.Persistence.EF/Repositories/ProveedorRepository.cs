using Retail.CheckList.Infrastructure.Persistence.EF.Storage;
using Retail.CheckList.Infrastructure.Repositories;
using Retail.CheckList.Model.Tables;

namespace Retail.CheckList.Infrastructure.Persistence.EF.Repositories
{
    public class ProveedorRepository : EFRepository<Proveedor>, IProveedorRepository
    {
        public ProveedorRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
