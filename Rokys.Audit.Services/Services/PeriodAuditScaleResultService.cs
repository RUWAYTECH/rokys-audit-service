using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.PeriodAuditScaleResult;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.PeriodAuditScaleResult;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class PeriodAuditScaleResultService : IPeriodAuditScaleResultService
    {
        private readonly IPeriodAuditScaleResultRepository _repository;
        private readonly IValidator<PeriodAuditScaleResultRequestDto> _validator;
        private readonly ILogger<PeriodAuditScaleResultService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPeriodAuditRepository _periodAuditRepository;
        private readonly IPeriodAuditGroupResultRepository _periodAuditGroupResultRepository;

        public PeriodAuditScaleResultService(
            IPeriodAuditScaleResultRepository repository,
            IValidator<PeriodAuditScaleResultRequestDto> validator,
            ILogger<PeriodAuditScaleResultService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IPeriodAuditRepository periodAuditRepository,
            IPeriodAuditGroupResultRepository periodAuditGroupResultRepository)
        {
            _repository = repository;
            _validator = validator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _periodAuditRepository = periodAuditRepository;
            _periodAuditGroupResultRepository = periodAuditGroupResultRepository;
        }

        public async Task<ResponseDto<PeriodAuditScaleResultResponseDto>> Create(PeriodAuditScaleResultRequestDto requestDto)
        {
            var response = ResponseDto.Create<PeriodAuditScaleResultResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<PeriodAuditScaleResult>(requestDto);
                entity.CreateAudit(currentUser.UserName);
                entity.IsActive = true;
                _repository.Insert(entity);
                await _unitOfWork.CommitAsync();
                var createdEntity = await _repository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditScaleResultId == entity.PeriodAuditScaleResultId && x.IsActive,
                    includeProperties: [x => x.PeriodAuditGroupResult, sg => sg.ScaleGroup]);
                response.Data = _mapper.Map<PeriodAuditScaleResultResponseDto>(createdEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PeriodAuditScaleResultResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditScaleResultId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error("No se encontró el registro.");
                    return response;
                }
                entity.IsActive = false;
                entity.UpdateDate = DateTime.UtcNow;
                _repository.Update(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PeriodAuditScaleResultResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<PeriodAuditScaleResultResponseDto>();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditScaleResultId == id && x.IsActive,
                    includeProperties: [ x => x.PeriodAuditGroupResult, sg => sg.ScaleGroup]);
                if (entity == null)
                {
                    response = ResponseDto.Error<PeriodAuditScaleResultResponseDto>("No se encontró el registro.");
                    return response;
                }
                response.Data = _mapper.Map<PeriodAuditScaleResultResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PeriodAuditScaleResultResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PeriodAuditScaleResultResponseDto>> Update(Guid id, PeriodAuditScaleResultRequestDto requestDto)
        {
            var response = ResponseDto.Create<PeriodAuditScaleResultResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var entity = await _repository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditScaleResultId == id && x.IsActive,
                    includeProperties: [x => x.PeriodAuditGroupResult, sg => sg.ScaleGroup]);
                if (entity == null)
                {
                    response = ResponseDto.Error<PeriodAuditScaleResultResponseDto>("No se encontró el registro.");
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                entity = _mapper.Map(requestDto, entity);
                entity.UpdateAudit(currentUser.UserName);
                _repository.Update(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<PeriodAuditScaleResultResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PeriodAuditScaleResultResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<PeriodAuditScaleResultResponseDto>>> GetPaged(PeriodAuditScaleResultFilterRequestDto filterRequestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<PeriodAuditScaleResultResponseDto>>();
            try
            {
                Expression<Func<PeriodAuditScaleResult, bool>> filter = x => x.IsActive;
                if (filterRequestDto.PeriodAuditGroupResultId.HasValue)
                {
                    filter = filter.AndAlso(x => x.PeriodAuditGroupResultId == filterRequestDto.PeriodAuditGroupResultId.Value);
                }
                if (filterRequestDto.ScaleGroupId.HasValue)
                {
                    filter = filter.AndAlso(x => x.ScaleGroupId == filterRequestDto.ScaleGroupId.Value);
                }
                if (!string.IsNullOrEmpty(filterRequestDto.Filter))
                {
                    filter = filter.AndAlso(x => (x.Observations != null && x.Observations.Contains(filterRequestDto.Filter)));
                }

                Func<IQueryable<PeriodAuditScaleResult>, IOrderedQueryable<PeriodAuditScaleResult>> orderBy = q => q.OrderByDescending(x => x.CreationDate);
                var entities = await _repository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: filterRequestDto.PageNumber,
                    pageSize: filterRequestDto.PageSize,
                    includeProperties: [ x => x.ScaleGroup, x => x.PeriodAuditGroupResult ]
                );
                var pagedResult = new PaginationResponseDto<PeriodAuditScaleResultResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<PeriodAuditScaleResultResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = filterRequestDto.PageNumber,
                    PageSize = filterRequestDto.PageSize
                };
                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PaginationResponseDto<PeriodAuditScaleResultResponseDto>>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PeriodAuditScaleResultCustomResponseDto>> GetByIdCustomData(Guid id)
        {
            var response = ResponseDto.Create<PeriodAuditScaleResultCustomResponseDto>();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditScaleResultId == id, 
                    includeProperties: [ e => e.PeriodAuditGroupResult.PeriodAudit.Store.Enterprise,
                        sg => sg.ScaleGroup,
                        a => a.PeriodAuditGroupResult.PeriodAudit.Administrator,
                        op => op.PeriodAuditGroupResult.PeriodAudit.OperationManager,
                        fa => fa.PeriodAuditGroupResult.PeriodAudit.FloatingAdministrator,
                        ra => ra.PeriodAuditGroupResult.PeriodAudit.ResponsibleAuditor,
                        asi => asi.PeriodAuditGroupResult.PeriodAudit.Assistant]);
                if (entity == null)
                {
                    response.Messages.Add(new ApplicationMessage { Message = "No se encontro la entidad", MessageType = ApplicationMessageType.Error });
                    return response;
                }
                var customDto = _mapper.Map<PeriodAuditScaleResultCustomResponseDto>(entity);
                response.Data = customDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response.Messages.Add(new ApplicationMessage { Message = ex.Message, MessageType = ApplicationMessageType.Error });
            }
            return response;
        }
    }
}
