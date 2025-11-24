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
        private readonly IStorageFilesRepository _storageFilesRepository;
        private readonly IValidator<GroupRequestDto> _fluentValidator;
        private readonly IValidator<GroupCloneRequestDto> _cloneValidator;
        private readonly ILogger<GroupService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly DTOs.Common.FileSettings _fileSettings;

        public GroupService(
            IGroupRepository groupRepository,
            IScaleGroupRepository scaleGroupRepository,
            ITableScaleTemplateRepository tableScaleTemplateRepository,
            IAuditTemplateFieldRepository auditTemplateFieldRepository,
            IScoringCriteriaRepository scoringCriteriaRepository,
            ICriteriaSubResultRepository criteriaSubResultRepository,
            IStorageFilesRepository storageFilesRepository,
            IValidator<GroupRequestDto> fluentValidator,
            IValidator<GroupCloneRequestDto> cloneValidator,
            ILogger<GroupService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            DTOs.Common.FileSettings fileSettings)
        {
            _groupRepository = groupRepository;
            _scaleGroupRepository = scaleGroupRepository;
            _tableScaleTemplateRepository = tableScaleTemplateRepository;
            _auditTemplateFieldRepository = auditTemplateFieldRepository;
            _scoringCriteriaRepository = scoringCriteriaRepository;
            _criteriaSubResultRepository = criteriaSubResultRepository;
            _storageFilesRepository = storageFilesRepository;
            _fluentValidator = fluentValidator;
            _cloneValidator = cloneValidator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _fileSettings = fileSettings;
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

                // Assign SortOrder server-side grouped by EnterpriseId
                var existingSortOrders = (await _groupRepository.GetAsync(filter: x => x.EnterpriseId == requestDto.EnterpriseId && x.IsActive))
                    .Select(x => x.SortOrder);
                entity.SortOrder = Rokys.Audit.Common.Helpers.SortOrderHelper.GetNextSortOrder(existingSortOrders);

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

                var scaleGroups = await _scaleGroupRepository.GetAsync(filter: x => x.GroupId == id && x.IsActive);
                if (scaleGroups != null && scaleGroups.Any())
                {
                    foreach (var scaleGroup in scaleGroups)
                    {
                        var criteriaSubResults = await _criteriaSubResultRepository.GetAsync(
                            filter: x => x.ScaleGroupId == scaleGroup.ScaleGroupId && x.IsActive);
                        if (criteriaSubResults.Any())
                        {
                            foreach (var criteria in criteriaSubResults)
                            {
                                criteria.IsActive = false;
                                _criteriaSubResultRepository.Update(criteria);
                            }
                            await _unitOfWork.CommitAsync();
                        }

                        var scoringCriterias = await _scoringCriteriaRepository.GetAsync(
                            filter: x => x.ScaleGroupId == scaleGroup.ScaleGroupId && x.IsActive);
                        if (scoringCriterias.Any())
                        {
                            foreach (var scoring in scoringCriterias)
                            {
                                scoring.IsActive = false;
                                _scoringCriteriaRepository.Update(scoring);
                            }
                            await _unitOfWork.CommitAsync();
                        }

                        var tableScaleTemplates = await _tableScaleTemplateRepository.GetAsync(
                            filter: x => x.ScaleGroupId == scaleGroup.ScaleGroupId && x.IsActive);
                        if (tableScaleTemplates.Any())
                        {
                            foreach (var template in tableScaleTemplates)
                            {
                                var auditTemplateFields = await _auditTemplateFieldRepository.GetAsync(
                                    filter: x => x.TableScaleTemplateId == template.TableScaleTemplateId && x.IsActive);
                                if (auditTemplateFields.Any())
                                {
                                    foreach (var field in auditTemplateFields)
                                    {
                                        field.IsActive = false;
                                        _auditTemplateFieldRepository.Update(field);
                                    }
                                    await _unitOfWork.CommitAsync();
                                }

                                template.IsActive = false;
                                _tableScaleTemplateRepository.Update(template);
                            }
                            await _unitOfWork.CommitAsync();
                        }

                        scaleGroup.IsActive = false;
                        _scaleGroupRepository.Update(scaleGroup);
                        await _unitOfWork.CommitAsync();
                    }
                }

                entity.IsActive = false;
                _groupRepository.Update(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error("Ocurrio un error al tratar de eliminar el Grupo");
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
                        SortOrder = originalScaleGroup.SortOrder,
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
                        SortOrder = originalTemplate.SortOrder,
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
                        SortOrder = originalField.SortOrder,
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

                // 8. Obtener y clonar archivos adjuntos de los ScaleGroups
                int storageFilesCount = 0;
                var originalStorageFiles = await _storageFilesRepository.GetAsync(
                    filter: x => scaleGroupMapping.Keys.Contains(x.EntityId) && x.IsActive);

                foreach (var originalFile in originalStorageFiles)
                {
                    // Clonar el archivo físico si existe
                    string? newFileUrl = null;
                    if (!string.IsNullOrEmpty(originalFile.FileUrl))
                    {
                        var originalFilePath = Path.Combine(_fileSettings.Path, "Uploads", originalFile.FileUrl);
                        if (File.Exists(originalFilePath))
                        {
                            var newFileName = $"{Guid.NewGuid()}_{Path.GetFileName(originalFile.FileName)}";
                            var uploadsFolder = Path.Combine(_fileSettings.Path, "Uploads");
                            if (!Directory.Exists(uploadsFolder))
                                Directory.CreateDirectory(uploadsFolder);

                            var newFilePath = Path.Combine(uploadsFolder, newFileName);
                            File.Copy(originalFilePath, newFilePath);
                            newFileUrl = newFileName;
                        }
                    }

                    // Crear el registro del archivo clonado
                    var clonedFile = new StorageFiles
                    {
                        StorageFileId = Guid.NewGuid(),
                        OriginalName = originalFile.OriginalName,
                        FileName = originalFile.FileName,
                        FileUrl = newFileUrl ?? originalFile.FileUrl,
                        FileType = originalFile.FileType,
                        UploadedBy = currentUser.UserName,
                        UploadDate = DateTime.Now,
                        ClassificationType = originalFile.ClassificationType,
                        EntityId = scaleGroupMapping[originalFile.EntityId],
                        EntityName = originalFile.EntityName,
                        IsActive = true
                    };
                    clonedFile.CreateAudit(currentUser.UserName);
                    _storageFilesRepository.Insert(clonedFile);
                    storageFilesCount++;
                }

                // 9. Guardar todos los cambios en una sola transacción
                await _unitOfWork.CommitAsync();

                // 10. Crear la respuesta
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
                    StorageFilesCloned = storageFilesCount,
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

        public async Task<ResponseDto<bool>> ChangeOrder(Guid enterpriseId, int currentPosition, int newPosition)
        {
            var response = ResponseDto.Create<bool>();
            try
            {
                var items = (await _groupRepository.GetAsync(filter: x => x.EnterpriseId == enterpriseId && x.IsActive))
                    .OrderBy(x => x.SortOrder)
                    .ToList();

                var currentIndex = items.FindIndex(x => x.SortOrder == currentPosition);
                var newIndex = items.FindIndex(x => x.SortOrder == newPosition);
                if (currentIndex < 0 || newIndex < 0)
                {
                    response = ResponseDto.Error<bool>("SortOrder no encontrado en la empresa.");
                    return response;
                }

                var item = items[currentIndex];
                items.RemoveAt(currentIndex);
                items.Insert(newIndex, item);

                for (int i = 0; i < items.Count; i++)
                {
                    items[i].SortOrder = i + 1;
                    _groupRepository.Update(items[i]);
                }
                await _unitOfWork.CommitAsync();
                response.Data = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<bool>(ex.Message);
            }
            return response;
        }
    }
}