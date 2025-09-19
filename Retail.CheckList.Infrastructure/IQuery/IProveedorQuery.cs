using Retail.CheckList.Model.Tables;

namespace Retail.CheckList.Infrastructure.IQuery
{
    public interface IProveedorQuery
    {
        Task<IEnumerable<Proveedor>> GetAllAsync();
    }
}
