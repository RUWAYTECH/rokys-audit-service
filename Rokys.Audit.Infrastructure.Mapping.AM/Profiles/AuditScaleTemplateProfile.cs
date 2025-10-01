using AutoMapper;
using Rokys.Audit.DTOs.Requests.AuditScaleTemplate;
using Rokys.Audit.DTOs.Responses.AuditScaleTemplate;
using Rokys.Audit.Model.Tables;

namespace Rokys.Audit.Infrastructure.Mapping.AM.Profiles
{
    public class AuditScaleTemplateProfile : Profile
    {
        public AuditScaleTemplateProfile()
        {
            CreateMap<AuditScaleTemplateRequestDto, AuditScaleTemplate>()
                .ForMember(dest => dest.AuditScaleTemplateId, opt => opt.Ignore())
                .ForMember(dest => dest.CreationDate, opt => opt.Ignore())
                .ForMember(dest => dest.UpdateDate, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
                .ForMember(dest => dest.IsActive, opt => opt.Ignore())
                .ForMember(dest => dest.AuditTemplateFields, opt => opt.Ignore());

            CreateMap<AuditScaleTemplate, AuditScaleTemplateResponseDto>()
                .ForMember(dest => dest.AuditScaleTemplateId, opt => opt.MapFrom(src => src.AuditScaleTemplateId))
                .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Code))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.TemplateData, opt => opt.MapFrom(src => src.TemplateData))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => src.CreationDate))
                .ForMember(dest => dest.UpdateDate, opt => opt.MapFrom(src => src.UpdateDate))
                .ForMember(dest => dest.CreatedBy, opt => opt.MapFrom(src => src.CreatedBy))
                .ForMember(dest => dest.UpdatedBy, opt => opt.MapFrom(src => src.UpdatedBy));
        }
    }
}