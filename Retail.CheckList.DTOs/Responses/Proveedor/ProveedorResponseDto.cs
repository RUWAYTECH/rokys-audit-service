using Retail.CheckList.DTOs.Common;

namespace Retail.CheckList.DTOs.Responses.Proveedor
{
    public class ProveedorResponseDto: ProveedorDto
    {
        public int IdProveedor { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreationDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdateDate { get; set; }
    }
}
