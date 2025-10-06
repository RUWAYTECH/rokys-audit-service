using AutoMapper;
using Rokys.Audit.DTOs.Requests.AuditScaleTemplate;
using Rokys.Audit.DTOs.Requests.AuditTemplateField;
using Rokys.Audit.DTOs.Requests.CriteriaSubResult;
using Rokys.Audit.DTOs.Requests.Enterprise;
using Rokys.Audit.DTOs.Requests.Group;
using Rokys.Audit.DTOs.Requests.Proveedor;
using Rokys.Audit.DTOs.Requests.ScaleCompany;
using Rokys.Audit.DTOs.Requests.ScaleGroup;
using Rokys.Audit.DTOs.Requests.ScoringCriteria;
using Rokys.Audit.DTOs.Requests.Store;
using Rokys.Audit.DTOs.Requests.TableScaleTemplate;
using Rokys.Audit.DTOs.Requests.PeriodAuditFieldValues;
using Rokys.Audit.DTOs.Requests.PeriodAudit;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Mapping.AM.Profiles
{
    public class DtoToModelProfile : Profile
    {
        public DtoToModelProfile()
        {
            CreateMap<ProveedorRequestDto, Proveedor>();
            CreateMap<ScaleCompanyRequestDto, ScaleCompany>();
            CreateMap<ScaleGroupRequestDto, ScaleGroup>();
            CreateMap<GroupRequestDto, Group>();
            CreateMap<CriteriaSubResultRequestDto, CriteriaSubResult>();
            CreateMap<PeriodAuditFieldValuesRequestDto, PeriodAuditFieldValues>();
            //CreateMap<AuditScaleTemplateRequestDto, AuditScaleTemplate>();
            CreateMap<TableScaleTemplateRequestDto, TableScaleTemplate>();
            CreateMap<EnterpriseRequestDto, Enterprise>();
            CreateMap<StoreRequestDto, Stores>();
            CreateMap<AuditTemplateFieldRequestDto, AuditTemplateFields>();
            CreateMap<ScoringCriteriaRequestDto, ScoringCriteria>();
                        CreateMap<PeriodAuditRequestDto, PeriodAudit>();
                    }
    }
}
