using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.IQuery
{
    public interface IProveedorQuery
    {
        Task<IEnumerable<Proveedor>> GetAllAsync();
    }
}
