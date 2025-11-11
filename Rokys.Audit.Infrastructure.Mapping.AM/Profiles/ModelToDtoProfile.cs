using AutoMapper;
using Rokys.Audit.DTOs.Responses.PeriodAuditGroupResult;
using Rokys.Audit.DTOs.Responses.StorageFiles;
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
using Rokys.Audit.DTOs.Responses.PeriodAuditScaleResult;
using Rokys.Audit.DTOs.Responses.PeriodAuditTableScaleTemplateResult;
using Rokys.Audit.DTOs.Responses.PeriodAuditScoringCriteriaResult;
using Rokys.Audit.DTOs.Responses.PeriodAuditScaleSubResult;
using Rokys.Audit.DTOs.Responses.InboxItems;
using Rokys.Audit.DTOs.Responses.Reports;
using Rokys.Audit.DTOs.Responses.AuditRoleConfiguration;

namespace Rokys.Audit.Infrastructure.Mapping.AM.Profiles
{
    public class ModelToDtoProfile : Profile
    {
        public ModelToDtoProfile()
        {
            CreateMap<PeriodAuditGroupResult, PeriodAuditGroupResultResponseDto>()
                .AfterMap((src, dest) =>
                {
                    dest.GroupName = src.Group != null ? src.Group.Name : string.Empty;
                });
            CreateMap<PeriodAuditScaleResult, PeriodAuditScaleResultResponseDto>()
                .AfterMap((src, dest) =>
                {
                    dest.ScaleGroup = src.ScaleGroup != null ? new ScaleGroupResponseDto
                    {
                        ScaleGroupId = src.ScaleGroup.ScaleGroupId,
                        GroupId = src.ScaleGroup.GroupId,
                        Name = src.ScaleGroup.Name,
                        Code = src.ScaleGroup.Code,
                        Weighting = src.ScaleGroup.Weighting,
                        IsActive = src.ScaleGroup.IsActive,
                    } : null;
                });
            CreateMap<Proveedor, ProveedorResponseDto>();
            CreateMap<ScaleCompany, ScaleCompanyResponseDto>()
                .AfterMap((src, dest) =>
                {
                    dest.EnterpriseName = src.Enterprise?.Name;
                });
            CreateMap<ScaleGroup, ScaleGroupResponseDto>();
            CreateMap<Group, GroupResponseDto>().AfterMap((src, dest) =>
            {
                dest.EnterpriseName = src.Enterprise?.Name;
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
            CreateMap<AuditRoleConfiguration, AuditRoleConfigurationResponseDto>();
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
                dest.SupervisorName = src.Supervisor?.FirstName + " " + src.Supervisor?.LastName;
                dest.StatusName = src.AuditStatus?.Name;
                dest.EnterpriseName = src.Store?.Enterprise?.Name ?? string.Empty;
                dest.EnterpriseId = src.Store?.EnterpriseId ?? Guid.Empty;
                dest.StoreName = src.Store?.Name ?? string.Empty;
                dest.ScaleCode = src.ScaleCode ?? string.Empty;

                if (src.AuditStatus != null)
                {
                    dest.AuditStatus = new AuditStatusResponseDto
                    {
                        AuditStatusId = src.AuditStatus.AuditStatusId,
                        Name = src.AuditStatus.Name,
                        ColorCode = src.AuditStatus.ColorCode,
                        Code = src.AuditStatus.Code,
                        IsActive = src.AuditStatus.IsActive,
                        CreatedBy = src.AuditStatus.CreatedBy,
                        CreationDate = src.AuditStatus.CreationDate,
                        UpdatedBy = src.AuditStatus.UpdatedBy,
                        UpdateDate = src.AuditStatus.UpdateDate
                    };
                }
            });
            CreateMap<AuditStatus, AuditStatusResponseDto>();
            CreateMap<UserReference, UserReferenceResponseDto>();
            CreateMap<EmployeeStore, EmployeeStoreResponseDto>();
            CreateMap<PeriodAuditTableScaleTemplateResult, PeriodAuditTableScaleTemplateResultResponseDto>();

            CreateMap<PeriodAuditTableScaleTemplateResult, PeriodAuditTableScaleTemplateResultListResponseDto>();
            CreateMap<StorageFiles, StorageFileResponseDto>();
            CreateMap<StorageFiles, StorageFileListResponseDto>();
            CreateMap<PeriodAuditFieldValues, PeriodAuditFieldValuesListResponseDto>();
            CreateMap<ScaleGroup, ScaleGroupPartialResponseDto>();
            CreateMap<PeriodAudit, PeriodAuditPartialResponseDto>();
            CreateMap<PeriodAuditScaleResult, PeriodAuditScaleResultCustomResponseDto>()
                .AfterMap((src, dest) =>
                {
                    dest.PeriodAudit = new PeriodAuditPartialResponseDto
                    {
                        PeriodAuditId = src.PeriodAuditGroupResult?.PeriodAudit?.PeriodAuditId ?? Guid.Empty,
                        StoreId = src.PeriodAuditGroupResult?.PeriodAudit?.Store?.StoreId ?? Guid.Empty,
                        StoreName = src.PeriodAuditGroupResult?.PeriodAudit?.Store?.Name ?? string.Empty,
                        EnterpriseId = src.PeriodAuditGroupResult?.PeriodAudit?.Store?.EnterpriseId ?? Guid.Empty,
                        EnterpriseName = src.PeriodAuditGroupResult?.PeriodAudit?.Store?.Enterprise?.Name ?? string.Empty,
                        AdministratorId = src.PeriodAuditGroupResult?.PeriodAudit?.AdministratorId ?? Guid.Empty,
                        AdministratorName = ($"{src.PeriodAuditGroupResult?.PeriodAudit?.Administrator?.FirstName} {src.PeriodAuditGroupResult?.PeriodAudit?.Administrator?.LastName}").Trim() ?? string.Empty,
                        AssistantId = src.PeriodAuditGroupResult?.PeriodAudit?.AssistantId ?? Guid.Empty,
                        AssistantName = ($"{src.PeriodAuditGroupResult?.PeriodAudit?.Assistant?.FirstName} {src.PeriodAuditGroupResult?.PeriodAudit?.Assistant?.LastName}").Trim() ?? string.Empty,
                        OperationManagersId = src.PeriodAuditGroupResult?.PeriodAudit?.OperationManagersId ?? Guid.Empty,
                        OperationManagerName = ($"{src.PeriodAuditGroupResult?.PeriodAudit?.OperationManager?.FirstName} {src.PeriodAuditGroupResult?.PeriodAudit?.OperationManager?.LastName}").Trim() ?? string.Empty,
                        FloatingAdministratorId = src.PeriodAuditGroupResult?.PeriodAudit?.FloatingAdministratorId ?? Guid.Empty,
                        FloatingAdministratorName = ($"{src.PeriodAuditGroupResult?.PeriodAudit?.FloatingAdministrator?.FirstName} {src.PeriodAuditGroupResult?.PeriodAudit?.FloatingAdministrator?.LastName}").Trim() ?? string.Empty,
                        ResponsibleAuditorId = src.PeriodAuditGroupResult?.PeriodAudit?.ResponsibleAuditorId ?? Guid.Empty,
                        ResponsibleAuditorName = ($"{src.PeriodAuditGroupResult?.PeriodAudit?.ResponsibleAuditor?.FirstName} {src.PeriodAuditGroupResult?.PeriodAudit?.ResponsibleAuditor?.LastName}").Trim() ?? string.Empty,
                        SupervisorId = src.PeriodAuditGroupResult?.PeriodAudit?.SupervisorId ?? Guid.Empty,
                        SupervisorName = ($"{src.PeriodAuditGroupResult?.PeriodAudit?.Supervisor?.FirstName} {src.PeriodAuditGroupResult?.PeriodAudit?.Supervisor?.LastName}").Trim() ?? string.Empty,
                        StatusId = src.PeriodAuditGroupResult?.PeriodAudit?.StatusId ?? Guid.Empty,
                        StartDate = src.PeriodAuditGroupResult?.PeriodAudit?.StartDate ?? default,
                        EndDate = src.PeriodAuditGroupResult?.PeriodAudit?.EndDate ?? default,
                        ReportDate = src.PeriodAuditGroupResult?.PeriodAudit?.ReportDate,
                        ScaleCode = src.PeriodAuditGroupResult?.PeriodAudit?.ScaleCode ?? string.Empty,
                        AuditStatus = src.PeriodAuditGroupResult.PeriodAudit.AuditStatus == null
                                        ? null
                                        : new AuditStatusResponseDto
                                        {
                                            AuditStatusId = src.PeriodAuditGroupResult.PeriodAudit.AuditStatus.AuditStatusId,
                                            Name = src.PeriodAuditGroupResult.PeriodAudit.AuditStatus.Name,
                                            Code = src.PeriodAuditGroupResult.PeriodAudit.AuditStatus.Code,
                                            ColorCode = src.PeriodAuditGroupResult.PeriodAudit.AuditStatus.ColorCode,
                                            IsActive = src.PeriodAuditGroupResult.PeriodAudit.AuditStatus.IsActive
                                        },
                        IsActive = src.PeriodAuditGroupResult?.PeriodAudit?.IsActive ?? false,
                    };
                    dest.ScaleCompany = src.PeriodAuditGroupResult?.PeriodAudit?.Store?.Enterprise?.ScaleCompanies
                        ?.Where(sc => sc.IsActive)
                        .Select(sc => new ScaleCompanyResponseDto
                        {
                            ScaleCompanyId = sc.ScaleCompanyId,
                            Name = sc.Name,
                            Code = sc.Code,
                            SortOrder = sc.SortOrder,
                            MinValue = sc.MinValue,
                            MaxValue = sc.MaxValue,
                            ColorCode = sc.ColorCode,
                            EnterpriseId = sc.EnterpriseId,
                            IsActive = sc.IsActive,
                            CreationDate = sc.CreationDate,
                            CreatedBy = sc.CreatedBy,
                        })
                        .ToList() ?? new List<ScaleCompanyResponseDto>();
                    dest.ScaleGroup = new ScaleGroupPartialResponseDto
                    {
                        ScaleGroupId = src.ScaleGroup.ScaleGroupId,
                        GroupId = src.ScaleGroup.GroupId,
                        Name = src.ScaleGroup.Name,
                        Code = src.ScaleGroup.Code,
                        HasSourceData = src.ScaleGroup.HasSourceData,
                        Weighting = src.ScaleGroup.Weighting,
                        IsActive = src.ScaleGroup.IsActive,
                    };
                    dest.PeriodAuditScaleSubResult = src.PeriodAuditScaleSubResults.Select(subResult => new PeriodAuditScaleSubResultResponseDto
                    {
                        PeriodAuditScaleSubResultId = subResult.PeriodAuditScaleSubResultId,
                        PeriodAuditScaleResultId = subResult.PeriodAuditScaleResultId,
                        CriteriaSubResultId = subResult.CriteriaSubResultId,
                        IsActive = subResult.IsActive,
                        CriteriaCode = subResult.CriteriaCode,
                        CriteriaName = subResult.CriteriaName,
                        EvaluatedValue = subResult.EvaluatedValue,
                        CalculatedResult = subResult.CalculatedResult,
                        AppliedFormula = subResult.AppliedFormula,
                        ScoreObtained = subResult.ScoreObtained,
                        ColorCode = subResult.ColorCode
                    }).ToList();
                    dest.PeriodAuditScoringCriteriaResult = src.PeriodAuditScoringCriteriaResults
                    .OrderBy(criteriaResult => criteriaResult.SortOrder)
                    .Select(criteriaResult => new PeriodAuditScoringCriteriaResultResponseDto
                    {
                        PeriodAuditScoringCriteriaResultId = criteriaResult.PeriodAuditScoringCriteriaResultId,
                        PeriodAuditScaleResultId = criteriaResult.PeriodAuditScaleResultId,
                        IsActive = criteriaResult.IsActive,
                        CriteriaCode = criteriaResult.CriteriaCode,
                        CriteriaName = criteriaResult.CriteriaName,
                        ResultFormula = criteriaResult.ResultFormula,
                        ComparisonOperator = criteriaResult.ComparisonOperator,
                        ExpectedValue = criteriaResult.ExpectedValue,
                        Score = criteriaResult.Score,
                        ResultObtained = criteriaResult.ResultObtained,
                        SortOrder = criteriaResult.SortOrder,

                    }).ToList();
                });
            CreateMap<PeriodAuditScoringCriteriaResult, PeriodAuditScoringCriteriaResultResponseDto>();
            CreateMap<PeriodAuditScaleSubResult, PeriodAuditScaleSubResultResponseDto>();
            CreateMap<InboxItems, InboxItemResponseDto>()
                .AfterMap((src, dest) =>
                {
                    dest.UserName = src.User?.FirstName + " " + src.User?.LastName;
                    if (src.NextStatus != null)
                    {
                        dest.NextStatus = new AuditStatusResponseDto
                        {
                            AuditStatusId = src.NextStatus.AuditStatusId,
                            Name = src.NextStatus.Name,
                            ColorCode = src.NextStatus.ColorCode,
                            Code = src.NextStatus.Code,
                            IsActive = src.NextStatus.IsActive,
                            CreatedBy = src.NextStatus.CreatedBy,
                            CreationDate = src.NextStatus.CreationDate,
                            UpdatedBy = src.NextStatus.UpdatedBy,
                            UpdateDate = src.NextStatus.UpdateDate
                        };
                    }
                });
            CreateMap<InboxItems, InboxItemListResponseDto>();

            CreateMap<PeriodAudit, PeriodAuditItemReportResponseDto>().AfterMap((src, dest) =>
            {
               dest.AdministratorName = src.Administrator?.FirstName + " " + src.Administrator?.LastName;
               dest.AssistantName = src.Assistant?.FirstName + " " + src.Assistant?.LastName;
               dest.OperationManagerName = src.OperationManager?.FirstName + " " + src.OperationManager?.LastName;
               dest.FloatingAdministratorName = src.FloatingAdministrator?.FirstName + " " + src.FloatingAdministrator?.LastName;
               dest.ResponsibleAuditorName = src.ResponsibleAuditor?.FirstName + " " + src.ResponsibleAuditor?.LastName;
               dest.SupervisorName = src.Supervisor?.FirstName + " " + src.Supervisor?.LastName;
               dest.EnterpriseName = src.Store?.Enterprise?.Name ?? string.Empty;
               dest.EnterpriseId = src.Store?.EnterpriseId ?? Guid.Empty;
               dest.StoreName = src.Store?.Name ?? string.Empty;

               if (src.AuditStatus != null)
               {
                   dest.AuditStatus = new AuditStatusResponseDto
                   {
                       AuditStatusId = src.AuditStatus.AuditStatusId,
                       Name = src.AuditStatus.Name,
                       ColorCode = src.AuditStatus.ColorCode,
                       Code = src.AuditStatus.Code,
                       IsActive = src.AuditStatus.IsActive,
                       CreatedBy = src.AuditStatus.CreatedBy,
                       CreationDate = src.AuditStatus.CreationDate,
                       UpdatedBy = src.AuditStatus.UpdatedBy,
                       UpdateDate = src.AuditStatus.UpdateDate
                   };
               }
            });
            
        }
    }
}
