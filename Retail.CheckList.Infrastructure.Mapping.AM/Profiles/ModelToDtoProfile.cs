using AutoMapper;
using Retail.CheckList.DTOs.Responses.Action;
using Retail.CheckList.DTOs.Responses.Proveedor;
using Retail.CheckList.Model.Tables;

namespace Retail.CheckList.Infrastructure.Mapping.AM.Profiles
{
    public class ModelToDtoProfile : Profile
    {
        public ModelToDtoProfile()
        {
            CreateMap<Proveedor, ProveedorResponseDto>();
        }
    }
}
