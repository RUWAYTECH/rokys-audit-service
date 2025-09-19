using AutoMapper;
using Retail.CheckList.DTOs.Requests.Proveedor;
using Retail.CheckList.Model.Tables;

namespace Retail.CheckList.Infrastructure.Mapping.AM.Profiles
{
    public class DtoToModelProfile : Profile
    {
        public DtoToModelProfile()
        {
           CreateMap<ProveedorRequestDto, Proveedor>();
        }
    }
}
