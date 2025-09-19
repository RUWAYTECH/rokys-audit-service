using Dapper.FluentMap.Mapping;
using Retail.CheckList.DTOs.Responses.Proveedor;

namespace Retail.CheckList.Infrastructure.Persistence.Dp.Mapping
{
    public class ProveedorMap : EntityMap<ProveedorResponseDto>
    {
        public ProveedorMap()
        {
            Map(a => a.IdProveedor).ToColumn("Id_Proveedor");
            Map(a => a.RUC).ToColumn("Ruc");
            Map(a => a.RazonSocial).ToColumn("Razon_Social");
        }
    }
}
