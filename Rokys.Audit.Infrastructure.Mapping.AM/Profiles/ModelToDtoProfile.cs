using AutoMapper;
using Rokys.Audit.DTOs.Responses.AuditScaleTemplate;
using Rokys.Audit.DTOs.Responses.AuditTemplateField;
using Rokys.Audit.DTOs.Responses.CriteriaSubResult;
using Rokys.Audit.DTOs.Responses.Enterprise;
using Rokys.Audit.DTOs.Responses.Group;
using Rokys.Audit.DTOs.Responses.MaintenanceTable;
using Rokys.Audit.DTOs.Responses.MaintenanceDetailTable;
using Rokys.Audit.DTOs.Responses.Proveedor;
using Rokys.Audit.DTOs.Responses.ScaleCompany;
using Rokys.Audit.DTOs.Responses.ScaleGroup;
using Rokys.Audit.DTOs.Responses.ScoringCriteria;
using Rokys.Audit.DTOs.Responses.Store;
using Rokys.Audit.DTOs.Responses.TableScaleTemplate;
using Rokys.Audit.DTOs.Responses.PeriodAuditFieldValues;
using Rokys.Audit.DTOs.Responses.PeriodAudit;
using Rokys.Audit.DTOs.Responses.UserReference;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.DTOs.Responses.AuditStatus;
using Rokys.Audit.DTOs.Responses.EmployeeStore;

namespace Rokys.Audit.Infrastructure.Mapping.AM.Profiles
{
    public class ModelToDtoProfile : Profile
    {
        public ModelToDtoProfile()
        {
            CreateMap<Proveedor, ProveedorResponseDto>();
            CreateMap<ScaleCompany, ScaleCompanyResponseDto>()
                .AfterMap((src, dest) =>
                {
                    dest.EnterpriseName = src.Enterprise.Name;
                });
            CreateMap<ScaleGroup, ScaleGroupResponseDto>();
            CreateMap<Group, GroupResponseDto>();
            CreateMap<CriteriaSubResult, CriteriaSubResultResponseDto>();
            CreateMap<PeriodAuditFieldValues, PeriodAuditFieldValuesResponseDto>();
            //CreateMap<AuditScaleTemplate, AuditScaleTemplateResponseDto>();
            CreateMap<TableScaleTemplate, TableScaleTemplateResponseDto>()
                .AfterMap((src, dest) =>
                {
                    dest.ScaleGroupName = src.ScaleGroup.Name;
                });
            CreateMap<Enterprise, EnterpriseResponseDto>();
            CreateMap<Stores, StoreResponseDto>();
            CreateMap<AuditTemplateFields, AuditTemplateFieldResponseDto>()
                .AfterMap((src, dest) =>
                {
                    dest.TableScaleTemplateName= src.TableScaleTemplate.Name;
                    dest.TableScaleTemplateCode = src.TableScaleTemplate.Code;
                });
            CreateMap<ScoringCriteria, ScoringCriteriaResponseDto>()
                .AfterMap((src, dest) =>
                {
                    dest.ScaleGroupName = src.ScaleGroup.Name;
                    dest.ScaleCalificationDescription = src.MaintenanceDetailTable.Description;
                });
            CreateMap<MaintenanceTable, MaintenanceTableResponseDto>();
            CreateMap<MaintenanceDetailTable, MaintenanceDetailTableResponseDto>();
            CreateMap<PeriodAudit, PeriodAuditResponseDto>();
            CreateMap<AuditStatus, AuditStatusResponseDto>();
            CreateMap<UserReference, UserReferenceResponseDto>();
            CreateMap<EmployeeStore, EmployeeStoreResponseDto>();
        }
    }
}
