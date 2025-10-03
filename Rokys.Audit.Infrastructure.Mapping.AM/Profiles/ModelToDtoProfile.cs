using AutoMapper;
using Rokys.Audit.DTOs.Responses.AuditScaleTemplate;
using Rokys.Audit.DTOs.Responses.AuditTemplateField;
using Rokys.Audit.DTOs.Responses.Enterprise;
using Rokys.Audit.DTOs.Responses.Group;
using Rokys.Audit.DTOs.Responses.Proveedor;
using Rokys.Audit.DTOs.Responses.ScaleCompany;
using Rokys.Audit.DTOs.Responses.ScaleGroup;
using Rokys.Audit.DTOs.Responses.Store;
using Rokys.Audit.DTOs.Responses.TableScaleTemplate;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Mapping.AM.Profiles
{
    public class ModelToDtoProfile : Profile
    {
        public ModelToDtoProfile()
        {
            CreateMap<Proveedor, ProveedorResponseDto>();
            CreateMap<ScaleCompany, ScaleCompanyResponseDto>();
            CreateMap<ScaleGroup, ScaleGroupResponseDto>();
            CreateMap<Group, GroupResponseDto>();
            //CreateMap<AuditScaleTemplate, AuditScaleTemplateResponseDto>();
            CreateMap<TableScaleTemplate, TableScaleTemplateResponseDto>().AfterMap((dest, src) =>
                dest.ScaleGroup.Name = src.ScaleGroupName);
            CreateMap<Enterprise, EnterpriseResponseDto>();
            CreateMap<Stores, StoreResponseDto>();
            CreateMap<AuditTemplateFields, AuditTemplateFieldResponseDto>();
        }
    }
}
