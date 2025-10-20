using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.Group;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.Group;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class GroupService : IGroupService
    {
        private readonly IGroupRepository _groupRepository;
        private readonly IScaleGroupRepository _scaleGroupRepository;
        private readonly ITableScaleTemplateRepository _tableScaleTemplateRepository;
        private readonly IAuditTemplateFieldRepository _auditTemplateFieldRepository;
        private readonly IScoringCriteriaRepository _scoringCriteriaRepository;
        private readonly ICriteriaSubResultRepository _criteriaSubResultRepository;
        private readonly IValidator<GroupRequestDto> _fluentValidator;
        private readonly IValidator<GroupCloneRequestDto> _cloneValidator;
        private readonly ILogger<GroupService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GroupService(
            IGroupRepository groupRepository,
            IScaleGroupRepository scaleGroupRepository,
            ITableScaleTemplateRepository tableScaleTemplateRepository,
            IAuditTemplateFieldRepository auditTemplateFieldRepository,
            IScoringCriteriaRepository scoringCriteriaRepository,
            ICriteriaSubResultRepository criteriaSubResultRepository,
            IValidator<GroupRequestDto> fluentValidator,
            IValidator<GroupCloneRequestDto> cloneValidator,
            ILogger<GroupService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor)
        {
            _groupRepository = groupRepository;
            _scaleGroupRepository = scaleGroupRepository;
            _tableScaleTemplateRepository = tableScaleTemplateRepository;
            _auditTemplateFieldRepository = auditTemplateFieldRepository;
            _scoringCriteriaRepository = scoringCriteriaRepository;
            _criteriaSubResultRepository = criteriaSubResultRepository;
            _fluentValidator = fluentValidator;
            _cloneValidator = cloneValidator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<GroupResponseDto>> Create(GroupRequestDto requestDto)
        {
            var response = ResponseDto.Create<GroupResponseDto>();
            try
            {
                var validate = _fluentValidator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<Group>(requestDto);
                entity.CreateAudit(currentUser.UserName);
                _groupRepository.Insert(entity);
                await _unitOfWork.CommitAsync();
                var createResponse = await _groupRepository.GetFirstOrDefaultAsync(filter: x => x.GroupId == entity.GroupId, includeProperties: [x => x.Enterprise]);
                response.Data = _mapper.Map<GroupResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<GroupResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _groupRepository.GetFirstOrDefaultAsync(filter: x => x.GroupId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error("No se encontró el grupo.");
                    return response;
                }
                entity.IsActive = false;
                _groupRepository.Update(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<GroupResponseDto>>> GetPaged(GroupFilterRequestDto groupFilterRequestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<GroupResponseDto>>();
            try
            {
                //var filter = BuildFilter(groupFilterRequestDto);
                Expression<Func<Group, bool>> filter = x => x.IsActive;
                if (!string.IsNullOrEmpty(groupFilterRequestDto.Filter))
                    filter = filter.AndAlso(x => x.Name.Contains(groupFilterRequestDto.Filter) && x.IsActive);

                if (groupFilterRequestDto.EnterpriseId.HasValue)
                    filter = filter.AndAlso(x => x.EnterpriseId == groupFilterRequestDto.EnterpriseId.Value);

                Func<IQueryable<Group>, IOrderedQueryable<Group>> orderBy = q => q.OrderByDescending(x => x.CreationDate);

                var entities = await _groupRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: groupFilterRequestDto.PageNumber,
                    pageSize: groupFilterRequestDto.PageSize,
                    includeProperties: [ x => x.Enterprise]
                );

                var pagedResult = new PaginationResponseDto<GroupResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<GroupResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = groupFilterRequestDto.PageNumber,
                    PageSize = groupFilterRequestDto.PageSize
                };

                response.Data = pagedResult;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PaginationResponseDto<GroupResponseDto>>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<GroupResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<GroupResponseDto>();
            try
            {
                var entity = await _groupRepository.GetFirstOrDefaultAsync(filter: x => x.GroupId == id && x.IsActive, includeProperties: [x => x.Enterprise]);
                if (entity == null)
                {
                    response = ResponseDto.Error<GroupResponseDto>("No se encontró el grupo.");
                    return response;
                }
                response.Data = _mapper.Map<GroupResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<GroupResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<GroupResponseDto>> Update(Guid id, GroupRequestDto requestDto)
        {
            var response = ResponseDto.Create<GroupResponseDto>();
            try
            {
                var validate = _fluentValidator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var entity = await _groupRepository.GetFirstOrDefaultAsync(filter: x => x.GroupId == id && x.IsActive, includeProperties: [x => x.Enterprise]);
                if (entity == null)
                {
                    response = ResponseDto.Error<GroupResponseDto>("No se encontró el grupo.");
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                entity = _mapper.Map(requestDto, entity);
                entity.UpdateAudit(currentUser.UserName);
                _groupRepository.Update(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<GroupResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<GroupResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<GroupCloneResponseDto>> CloneGroupAsync(GroupCloneRequestDto requestDto)
        {
            var response = ResponseDto.Create<GroupCloneResponseDto>();
            try
            {
                var validate = _cloneValidator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }

                var currentUser = _httpContextAccessor.CurrentUser();

                // 1. Obtener el grupo original con todas sus entidades hijas
                var originalGroup = await _groupRepository.GetFirstOrDefaultAsync(
                    filter: x => x.GroupId == requestDto.GroupId && x.IsActive,
                    includeProperties: [x => x.Enterprise]);

                if (originalGroup == null)
                {
                    response = ResponseDto.Error<GroupCloneResponseDto>("No se encontró el grupo a clonar.");
                    return response;
                }

                // 2. Crear el nuevo grupo clonado
                var clonedGroup = new Group
                {
                    GroupId = Guid.NewGuid(),
                    EnterpriseId = requestDto.EnterpriseId,
                    Name = requestDto.Name,
                    Weighting = requestDto.Weighting,
                    IsActive = true
                };
                clonedGroup.CreateAudit(currentUser.UserName);
                _groupRepository.Insert(clonedGroup);

                // Contadores para la respuesta
                int scaleGroupsCount = 0;
                int tableScaleTemplatesCount = 0;
                int auditTemplateFieldsCount = 0;
                int scoringCriteriaCount = 0;
                int criteriaSubResultsCount = 0;

                // 3. Obtener y clonar ScaleGroups
                var originalScaleGroups = await _scaleGroupRepository.GetAsync(
                    filter: x => x.GroupId == requestDto.GroupId && x.IsActive);

                var scaleGroupMapping = new Dictionary<Guid, Guid>(); // Original ID -> Cloned ID

                foreach (var originalScaleGroup in originalScaleGroups)
                {
                    var clonedScaleGroup = new ScaleGroup
                    {
                        ScaleGroupId = Guid.NewGuid(),
                        GroupId = clonedGroup.GroupId,
                        Code = originalScaleGroup.Code,
                        Name = originalScaleGroup.Name,
                        Weighting = originalScaleGroup.Weighting,
                        IsActive = originalScaleGroup.IsActive
                    };
                    clonedScaleGroup.CreateAudit(currentUser.UserName);
                    _scaleGroupRepository.Insert(clonedScaleGroup);
                    
                    scaleGroupMapping[originalScaleGroup.ScaleGroupId] = clonedScaleGroup.ScaleGroupId;
                    scaleGroupsCount++;
                }

                // 4. Obtener y clonar TableScaleTemplates
                var originalTableScaleTemplates = await _tableScaleTemplateRepository.GetAsync(
                    filter: x => scaleGroupMapping.Keys.Contains(x.ScaleGroupId) && x.IsActive);

                var tableScaleTemplateMapping = new Dictionary<Guid, Guid>(); // Original ID -> Cloned ID

                foreach (var originalTemplate in originalTableScaleTemplates)
                {
                    var clonedTemplate = new TableScaleTemplate
                    {
                        TableScaleTemplateId = Guid.NewGuid(),
                        ScaleGroupId = scaleGroupMapping[originalTemplate.ScaleGroupId],
                        Code = originalTemplate.Code,
                        Name = originalTemplate.Name,
                        Orientation = originalTemplate.Orientation,
                        TemplateData = originalTemplate.TemplateData,
                        IsActive = originalTemplate.IsActive
                    };
                    clonedTemplate.CreateAudit(currentUser.UserName);
                    _tableScaleTemplateRepository.Insert(clonedTemplate);
                    
                    tableScaleTemplateMapping[originalTemplate.TableScaleTemplateId] = clonedTemplate.TableScaleTemplateId;
                    tableScaleTemplatesCount++;
                }

                // 5. Obtener y clonar AuditTemplateFields
                var originalAuditTemplateFields = await _auditTemplateFieldRepository.GetAsync(
                    filter: x => tableScaleTemplateMapping.Keys.Contains(x.TableScaleTemplateId) && x.IsActive);

                foreach (var originalField in originalAuditTemplateFields)
                {
                    var clonedField = new AuditTemplateFields
                    {
                        AuditTemplateFieldId = Guid.NewGuid(),
                        TableScaleTemplateId = tableScaleTemplateMapping[originalField.TableScaleTemplateId],
                        FieldCode = originalField.FieldCode,
                        FieldName = originalField.FieldName,
                        FieldType = originalField.FieldType,
                        IsCalculated = originalField.IsCalculated,
                        CalculationFormula = originalField.CalculationFormula,
                        AcumulationType = originalField.AcumulationType,
                        FieldOptions = originalField.FieldOptions,
                        DefaultValue = originalField.DefaultValue,
                        IsActive = originalField.IsActive
                    };
                    clonedField.CreateAudit(currentUser.UserName);
                    _auditTemplateFieldRepository.Insert(clonedField);
                    auditTemplateFieldsCount++;
                }

                // 6. Obtener y clonar ScoringCriteria
                var originalScoringCriteria = await _scoringCriteriaRepository.GetAsync(
                    filter: x => scaleGroupMapping.Keys.Contains(x.ScaleGroupId) && x.IsActive);

                foreach (var originalCriteria in originalScoringCriteria)
                {
                    var clonedCriteria = new ScoringCriteria
                    {
                        ScoringCriteriaId = Guid.NewGuid(),
                        ScaleGroupId = scaleGroupMapping[originalCriteria.ScaleGroupId],
                        ScaleCalificationId = originalCriteria.ScaleCalificationId,
                        CriteriaName = originalCriteria.CriteriaName,
                        CriteriaCode = originalCriteria.CriteriaCode,
                        ResultFormula = originalCriteria.ResultFormula,
                        ComparisonOperator = originalCriteria.ComparisonOperator,
                        ExpectedValue = originalCriteria.ExpectedValue,
                        Score = originalCriteria.Score,
                        SortOrder = originalCriteria.SortOrder,
                        ErrorMessage = originalCriteria.ErrorMessage,
                        SuccessMessage = originalCriteria.SuccessMessage,
                        IsActive = originalCriteria.IsActive
                    };
                    clonedCriteria.CreateAudit(currentUser.UserName);
                    _scoringCriteriaRepository.Insert(clonedCriteria);
                    scoringCriteriaCount++;
                }

                // 7. Obtener y clonar CriteriaSubResult
                var originalCriteriaSubResults = await _criteriaSubResultRepository.GetAsync(
                    filter: x => scaleGroupMapping.Keys.Contains(x.ScaleGroupId) && x.IsActive);

                foreach (var originalSubResult in originalCriteriaSubResults)
                {
                    var clonedSubResult = new CriteriaSubResult
                    {
                        CriteriaSubResultId = Guid.NewGuid(),
                        ScaleGroupId = scaleGroupMapping[originalSubResult.ScaleGroupId],
                        CriteriaName = originalSubResult.CriteriaName,
                        CriteriaCode = originalSubResult.CriteriaCode,
                        ResultFormula = originalSubResult.ResultFormula,
                        ColorCode = originalSubResult.ColorCode,
                        Score = originalSubResult.Score,
                        IsActive = originalSubResult.IsActive
                    };
                    clonedSubResult.CreateAudit(currentUser.UserName);
                    _criteriaSubResultRepository.Insert(clonedSubResult);
                    criteriaSubResultsCount++;
                }

                // 8. Guardar todos los cambios en una sola transacción
                await _unitOfWork.CommitAsync();

                // 9. Crear la respuesta
                response.Data = new GroupCloneResponseDto
                {
                    OriginalGroupId = requestDto.GroupId,
                    ClonedGroupId = clonedGroup.GroupId,
                    EnterpriseId = requestDto.EnterpriseId,
                    GroupName = clonedGroup.Name,
                    ScaleGroupsCloned = scaleGroupsCount,
                    TableScaleTemplatesCloned = tableScaleTemplatesCount,
                    AuditTemplateFieldsCloned = auditTemplateFieldsCount,
                    ScoringCriteriaCloned = scoringCriteriaCount,
                    CriteriaSubResultsCloned = criteriaSubResultsCount,
                    ClonedAt = DateTime.Now
                };

                _logger.LogInformation($"Grupo clonado exitosamente. Original: {requestDto.GroupId}, Clonado: {clonedGroup.GroupId}");

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al clonar el grupo {requestDto.GroupId}: {ex.Message}");
                response = ResponseDto.Error<GroupCloneResponseDto>(ex.Message);
            }
            return response;
        }
    }
}