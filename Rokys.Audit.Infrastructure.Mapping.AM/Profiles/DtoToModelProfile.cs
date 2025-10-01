using AutoMapper;
using Rokys.Audit.DTOs.Requests.Proveedor;
using Rokys.Audit.DTOs.Requests.ScaleCompany;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Mapping.AM.Profiles
{
    public class DtoToModelProfile : Profile
    {
        public DtoToModelProfile()
        {
           CreateMap<ProveedorRequestDto, Proveedor>();
           CreateMap<ScaleCompanyRequestDto, ScaleCompany>();
        }
    }
}
