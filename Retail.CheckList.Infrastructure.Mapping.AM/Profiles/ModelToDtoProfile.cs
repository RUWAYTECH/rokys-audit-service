using AutoMapper;
using Rokys.Audit.DTOs.Responses.Action;
using Rokys.Audit.DTOs.Responses.Proveedor;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Mapping.AM.Profiles
{
    public class ModelToDtoProfile : Profile
    {
        public ModelToDtoProfile()
        {
            CreateMap<Proveedor, ProveedorResponseDto>();
        }
    }
}
