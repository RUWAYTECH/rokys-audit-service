using Retail.CheckList.DTOs.Requests.Proveedor;
using Retail.CheckList.DTOs.Responses.Common;
using Retail.CheckList.DTOs.Responses.Proveedor;

namespace Retail.CheckList.Services.Interfaces
{
    public interface IProveedorService: IBaseService<ProveedorRequestDto, ProveedorResponseDto>
    {
        public Task<ResponseDto<ProveedorResponseDto>> GetByRuc(string ruc);
    }
}
