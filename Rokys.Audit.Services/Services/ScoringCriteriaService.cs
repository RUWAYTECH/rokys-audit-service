using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.ScoringCriteria;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.ScoringCriteria;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class ScoringCriteriaService : IScoringCriteriaService
    {
        private readonly IScoringCriteriaRepository _scoringCriteriaRepository;
        private readonly IValidator<ScoringCriteriaRequestDto> _fluentValidator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ScoringCriteriaService(IScoringCriteriaRepository scoringCriteriaRepository,
            IValidator<ScoringCriteriaRequestDto> fluentValidator,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            ILogger<ScoringCriteriaService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _scoringCriteriaRepository = scoringCriteriaRepository;
            _fluentValidator = fluentValidator;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ResponseDto<ScoringCriteriaResponseDto>> Create(ScoringCriteriaRequestDto requestDto)
        {
            var response = ResponseDto.Create<ScoringCriteriaResponseDto>();
            try
            {
                var validate = _fluentValidator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                // Obtener el último código existente
                var lastCode = _scoringCriteriaRepository.GetFirstOrDefault(orderBy: q => q.OrderByDescending(x => x.CreationDate))?.CriteriaCode;
                var nextCode = Rokys.Audit.Common.Helpers.CodeGeneratorHelper.GenerateNextCode("SR", lastCode, 4);
                var entity = _mapper.Map<ScoringCriteria>(requestDto);
                entity.CriteriaCode = nextCode;
                // Obtener el siguiente sortOrder para el grupo
                var existingSortOrders = _scoringCriteriaRepository
                    .Get(x => x.ScaleGroupId == entity.ScaleGroupId)
                    .Select(x => x.SortOrder);
                entity.SortOrder = Rokys.Audit.Common.Helpers.SortOrderHelper.GetNextSortOrder(existingSortOrders);
                entity.CreateAudit(currentUser.UserName);
                // Insertar y guardar cambios
                _scoringCriteriaRepository.Insert(entity);
                await _unitOfWork.CommitAsync();
                var responseEntity = await _scoringCriteriaRepository.GetFirstOrDefaultAsync(
                    filter: x => x.ScoringCriteriaId == entity.ScoringCriteriaId,
                    includeProperties: [at => at.ScaleGroup, mdt => mdt.MaintenanceDetailTable]);
                response.Data = _mapper.Map<ScoringCriteriaResponseDto>(responseEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                if (ex.Message.Contains("UNIQUE"))
                {
                    response = ResponseDto.Error<ScoringCriteriaResponseDto>("No se pudo generar un código único. Intente nuevamente.");
                }
                else
                {
                    response = ResponseDto.Error<ScoringCriteriaResponseDto>(ex.Message);
                }
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _scoringCriteriaRepository.GetFirstOrDefaultAsync(
                    filter: x => x.ScoringCriteriaId == id && x.IsActive,
                    includeProperties: [at => at.ScaleGroup, mdt => mdt.MaintenanceDetailTable]);
                if (entity == null)
                {
                    response = ResponseDto.Error("No se encontro el criterio de puntuación.");
                    return response;
                }
                entity.IsActive = false;
                _scoringCriteriaRepository.Delete(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<ScoringCriteriaResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<ScoringCriteriaResponseDto>();
            try
            {
                var entity = _scoringCriteriaRepository.GetFirstOrDefault(
                    filter: x => x.ScoringCriteriaId == id && x.IsActive,
                    includeProperties: [at => at.ScaleGroup, mdt => mdt.MaintenanceDetailTable]);
                if (entity == null)
                {
                    response = ResponseDto.Error<ScoringCriteriaResponseDto>("No se encontro el criterio de puntuación.");
                    return response;
                }
                response.Data = _mapper.Map<ScoringCriteriaResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<ScoringCriteriaResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<ScoringCriteriaResponseDto>>> GetPaged(ScoringCriteriaFilterRequestDto requestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<ScoringCriteriaResponseDto>>();
            try
            {
                Expression<Func<ScoringCriteria, bool>> filter = x => x.IsActive;

                Func<IQueryable<ScoringCriteria>, IOrderedQueryable<ScoringCriteria>> orderBy = q => q.OrderByDescending(x => x.CreationDate);

                if (requestDto.ScaleGroupId.HasValue)
                    filter = x => x.ScaleGroupId == requestDto.ScaleGroupId.Value && x.IsActive;

                var entities = await _scoringCriteriaRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: requestDto.PageNumber,
                    pageSize: requestDto.PageSize,
                    includeProperties: [x => x.ScaleGroup, mdt => mdt.MaintenanceDetailTable]
                );

                var pagedResult = new PaginationResponseDto<ScoringCriteriaResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<ScoringCriteriaResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = requestDto.PageNumber,
                    PageSize = requestDto.PageSize
                };

                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PaginationResponseDto<ScoringCriteriaResponseDto>>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<ScoringCriteriaResponseDto>> Update(Guid id, ScoringCriteriaRequestDto requestDto)
        {
            var response = ResponseDto.Create<ScoringCriteriaResponseDto>();
            try
            {
                var validationResult = _fluentValidator.Validate(requestDto);
                if (!validationResult.IsValid)
                {
                    foreach (var error in validationResult.Errors)
                    {
                        response.Messages.Add(new ApplicationMessage
                        {
                            Message = error.ErrorMessage,
                            MessageType = ApplicationMessageType.Error
                        });
                    }
                    return response;
                }
                var entity = await _scoringCriteriaRepository.GetFirstOrDefaultAsync(filter: x => x.ScoringCriteriaId == id && x.IsActive, includeProperties: [x => x.ScaleGroup, mdt => mdt.MaintenanceDetailTable]);
                if (entity == null)
                {
                    response = ResponseDto.Error<ScoringCriteriaResponseDto>("No se encontro el criterio de puntuación.");
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                entity = _mapper.Map(requestDto, entity);
                entity.UpdateAudit(currentUser.UserName);
                _scoringCriteriaRepository.Update(entity);
                await _unitOfWork.CommitAsync();
                var responseData = _mapper.Map<ScoringCriteriaResponseDto>(entity);
                response = ResponseDto.Create(responseData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<ScoringCriteriaResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<bool>> ChangeOrder(Guid scaleGroupId, int currentSortOrder, int newSortOrder)
        {
            var response = ResponseDto.Create<bool>();
            try
            {
                var items = _scoringCriteriaRepository
                    .Get(x => x.ScaleGroupId == scaleGroupId && x.IsActive)
                    .OrderBy(x => x.SortOrder)
                    .ToList();

                var currentIndex = items.FindIndex(x => x.SortOrder == currentSortOrder);
                var newIndex = items.FindIndex(x => x.SortOrder == newSortOrder);

                if (currentIndex == -1 || newIndex == -1)
                {
                    response = ResponseDto.Error<bool>("SortOrder no encontrado en el grupo.");
                    return response;
                }

                var movedItem = items[currentIndex];
                items.RemoveAt(currentIndex);
                items.Insert(newIndex, movedItem);

                // Actualizar el SortOrder de todos
                for (int i = 0; i < items.Count; i++)
                {
                    items[i].SortOrder = i + 1;
                    _scoringCriteriaRepository.Update(items[i]);
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

        private Expression<Func<ScoringCriteria, bool>> BuildFilter(ScoringCriteriaFilterRequestDto dto)
        {
            return x =>
                x.IsActive &&
                (!dto.ScaleGroupId.HasValue || x.ScaleGroupId == dto.ScaleGroupId.Value) &&
                (string.IsNullOrEmpty(dto.Filter) || x.CriteriaName.Contains(dto.Filter) || x.CriteriaCode.Contains(dto.Filter));
        }
    }
}
