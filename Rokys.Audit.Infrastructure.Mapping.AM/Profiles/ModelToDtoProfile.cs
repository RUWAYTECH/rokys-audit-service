using AutoMapper;
using Rokys.Audit.DTOs.Responses.PeriodAuditGroupResult;
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
            CreateMap<PeriodAuditGroupResult, PeriodAuditGroupResultResponseDto>()
                .AfterMap((src, dest) =>
                {
                    dest.ScaleDescription = src.ScaleDescription;
                    dest.Observations = src.Observations;
                });
            CreateMap<Proveedor, ProveedorResponseDto>();
            CreateMap<ScaleCompany, ScaleCompanyResponseDto>()
                .AfterMap((src, dest) =>
                {
                    dest.EnterpriseName = src.Enterprise.Name;
                });
            CreateMap<ScaleGroup, ScaleGroupResponseDto>();
            CreateMap<Group, GroupResponseDto>().AfterMap((src, dest) =>
            {
                dest.EnterpriseName = src.Enterprise.Name;
            });
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
            CreateMap<PeriodAudit, PeriodAuditResponseDto>().AfterMap((src, dest) =>
            {
                dest.AdministratorName = src.Administrator?.FirstName + " " + src.Administrator?.LastName;
                dest.AssistantName = src.Assistant?.FirstName + " " + src.Assistant?.LastName;
                dest.OperationManagerName = src.OperationManager?.FirstName + " " + src.OperationManager?.LastName;
                dest.FloatingAdministratorName = src.FloatingAdministrator?.FirstName + " " + src.FloatingAdministrator?.LastName;
                dest.ResponsibleAuditorName = src.ResponsibleAuditor?.FirstName + " " + src.ResponsibleAuditor?.LastName;
                dest.StatusName = src.AuditStatus?.Name;
                dest.EnterpriseName = src.Store?.Enterprise?.Name ?? string.Empty;
                dest.EnterpriseId = src.Store?.EnterpriseId ?? Guid.Empty;
                dest.StoreName = src.Store?.Name ?? string.Empty;
            });
            CreateMap<AuditStatus, AuditStatusResponseDto>();
            CreateMap<UserReference, UserReferenceResponseDto>();
            CreateMap<EmployeeStore, EmployeeStoreResponseDto>();
        }
    }
}
