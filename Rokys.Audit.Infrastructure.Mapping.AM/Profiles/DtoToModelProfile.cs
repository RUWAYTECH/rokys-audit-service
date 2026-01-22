using AutoMapper;
using Rokys.Audit.DTOs.Requests.PeriodAuditGroupResult;
using Rokys.Audit.DTOs.Requests.AuditScaleTemplate;
using Rokys.Audit.DTOs.Requests.PeriodAuditTableScaleTemplateResult;
using Rokys.Audit.DTOs.Requests.AuditTemplateField;
using Rokys.Audit.DTOs.Requests.CriteriaSubResult;
using Rokys.Audit.DTOs.Requests.Enterprise;
using Rokys.Audit.DTOs.Requests.Group;
using Rokys.Audit.DTOs.Requests.MaintenanceTable;
using Rokys.Audit.DTOs.Requests.MaintenanceDetailTable;
using Rokys.Audit.DTOs.Requests.Proveedor;
using Rokys.Audit.DTOs.Requests.ScaleCompany;
using Rokys.Audit.DTOs.Requests.ScaleGroup;
using Rokys.Audit.DTOs.Requests.ScoringCriteria;
using Rokys.Audit.DTOs.Requests.Store;
using Rokys.Audit.DTOs.Requests.TableScaleTemplate;
using Rokys.Audit.DTOs.Requests.PeriodAuditFieldValues;
using Rokys.Audit.DTOs.Requests.PeriodAudit;
using Rokys.Audit.DTOs.Requests.UserReference;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.DTOs.Requests.AuditStatus;
using Rokys.Audit.DTOs.Requests.EmployeeStore;
using Rokys.Audit.DTOs.Requests.PeriodAuditScaleResult;
using Rokys.Audit.DTOs.Requests.StorageFiles;
using Rokys.Audit.DTOs.Requests.InboxItems;
using Rokys.Audit.DTOs.Requests.AuditRoleConfiguration;
using Rokys.Audit.DTOs.Requests.SubstitutionHistory;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.Infrastructure.Mapping.AM.Profiles
{
    public class DtoToModelProfile : Profile
    {
        public DtoToModelProfile()
        {
            CreateMap<PeriodAuditGroupResultRequestDto, PeriodAuditGroupResult>();
            CreateMap<PeriodAuditScaleResultRequestDto, PeriodAuditScaleResult>();
            CreateMap<ProveedorRequestDto, Proveedor>();
            CreateMap<ScaleCompanyRequestDto, ScaleCompany>();
            CreateMap<ScaleGroupRequestDto, ScaleGroup>();
            CreateMap<GroupRequestDto, Group>();
            CreateMap<CriteriaSubResultRequestDto, CriteriaSubResult>();
            //CreateMap<AuditScaleTemplateRequestDto, AuditScaleTemplate>();
            CreateMap<TableScaleTemplateRequestDto, TableScaleTemplate>();
            CreateMap<EnterpriseRequestDto, Enterprise>();
            CreateMap<StoreRequestDto, Stores>();
            CreateMap<AuditTemplateFieldRequestDto, AuditTemplateFields>();
            CreateMap<ScoringCriteriaRequestDto, ScoringCriteria>();
            CreateMap<MaintenanceTableRequestDto, MaintenanceTable>();
            CreateMap<MaintenanceDetailTableRequestDto, MaintenanceDetailTable>();
            CreateMap<PeriodAuditRequestDto, PeriodAudit>();
            CreateMap<AuditStatusRequestDto, AuditStatus>();
            CreateMap<UserReferenceRequestDto, UserReference>();
            CreateMap<EmployeeStoreRequestDto, EmployeeStore>();
            CreateMap<PeriodAuditTableScaleTemplateResultRequestDto, PeriodAuditTableScaleTemplateResult>();
            CreateMap<PeriodAuditFieldValuesUpdateAllValuesRequestDto, PeriodAuditFieldValues>();
            CreateMap<UpdatePeriodAuditFieldValuesRequestDto, PeriodAuditFieldValues>();
            CreateMap<StorageFileRequestDto, StorageFiles>();
            CreateMap<InboxItemRequestDto, InboxItems>();
            CreateMap<PeriodAuditParticipantDto, PeriodAuditParticipant>();
            CreateMap<AuditRoleConfigurationRequestDto, AuditRoleConfiguration>();
            CreateMap<SubstitutionHistoryRequestDto, SubstitutionHistory>();
        }
    }
}
