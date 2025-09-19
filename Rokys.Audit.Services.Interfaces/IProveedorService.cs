using Rokys.Audit.DTOs.Requests.Proveedor;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.Proveedor;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IProveedorService: IBaseService<ProveedorRequestDto, ProveedorResponseDto>
    {
        public Task<ResponseDto<ProveedorResponseDto>> GetByRuc(string ruc);
    }
}
