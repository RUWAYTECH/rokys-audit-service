using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Constant;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.PeriodAudit;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.PeriodAudit;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class PeriodAuditService : IPeriodAuditService
    {
        private readonly IRepository<PeriodAudit> _repository;
        private readonly IValidator<PeriodAuditRequestDto> _validator;
        private readonly ILogger<PeriodAuditService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuditStatusRepository _auditStatusRepository;
        private readonly IPeriodAuditGroupResultRepository _periodAuditGroupResultRepository;
        private readonly IScaleCompanyRepository _scaleCompanyRepository;

        public PeriodAuditService(
            IRepository<PeriodAudit> repository,
            IValidator<PeriodAuditRequestDto> validator,
            ILogger<PeriodAuditService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IAuditStatusRepository auditStatusRepository,
            IPeriodAuditGroupResultRepository periodAuditGroupResultRepository,
            IScaleCompanyRepository scaleCompanyRepository)
        {
            _repository = repository;
            _validator = validator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _auditStatusRepository = auditStatusRepository;
            _periodAuditGroupResultRepository = periodAuditGroupResultRepository;
            _scaleCompanyRepository = scaleCompanyRepository;
        }

        public async Task<ResponseDto<PeriodAuditResponseDto>> Create(PeriodAuditRequestDto requestDto)
        {
            var response = ResponseDto.Create<PeriodAuditResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }

                var auditStatus = await _auditStatusRepository.GetFirstOrDefaultAsync(filter: x => x.Code == AuditStatusCode.Pending && x.IsActive);

                var currentUser = _httpContextAccessor.CurrentUser();
                // Obtener el último código existente
                var lastCode = _repository.Get()
                    .OrderByDescending(x => x.CreationDate)
                    .Select(x => x.CorrelativeNumber)
                    .FirstOrDefault();
                var currentYear = DateTime.Now.Year;
                var nextCode = Rokys.Audit.Common.Helpers.CodeGeneratorHelper.GenerateNextCode("CSR-" + currentYear, lastCode, 4);
                requestDto.CorrelativeNumber = nextCode;
                var entity = _mapper.Map<PeriodAudit>(requestDto);
                entity.StatusId = auditStatus.AuditStatusId;
                entity.CreateAudit(currentUser.UserName);
                _repository.Insert(entity);
                await _unitOfWork.CommitAsync();
                var entityCreate = await _repository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditId == entity.PeriodAuditId && x.IsActive,
                    includeProperties: [x => x.Store.Enterprise, x => x.Administrator, x => x.Assistant, x => x.OperationManager, x => x.FloatingAdministrator, x => x.ResponsibleAuditor, x => x.AuditStatus, su => su.Supervisor]);
                response.Data = _mapper.Map<PeriodAuditResponseDto>(entityCreate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PeriodAuditResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error("No se encontró el registro.");
                    return response;
                }
                entity.IsActive = false;
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

        public async Task<ResponseDto<PeriodAuditResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<PeriodAuditResponseDto>();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditId == id && x.IsActive,
                    includeProperties: [x => x.Store.Enterprise, x => x.Administrator, x => x.Assistant, x => x.OperationManager, x => x.FloatingAdministrator, x => x.ResponsibleAuditor, x => x.AuditStatus, su => su.Supervisor]);
                if (entity == null)
                {
                    response = ResponseDto.Error<PeriodAuditResponseDto>("No se encontró el registro.");
                    return response;
                }
                response.Data = _mapper.Map<PeriodAuditResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PeriodAuditResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PeriodAuditResponseDto>> Update(Guid id, PeriodAuditRequestDto requestDto)
        {
            var response = ResponseDto.Create<PeriodAuditResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var entity = await _repository.GetFirstOrDefaultAsync(
                    filter: x => x.PeriodAuditId == id && x.IsActive, 
                    includeProperties: [x => x.Store.Enterprise, x => x.Administrator, x => x.Assistant, x => x.OperationManager, x => x.FloatingAdministrator, x => x.ResponsibleAuditor, x => x.AuditStatus, su => su.Supervisor]);
                if (entity == null)
                {
                    response = ResponseDto.Error<PeriodAuditResponseDto>("No se encontró el registro.");
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                entity = _mapper.Map(requestDto, entity);
                entity.UpdateAudit(currentUser.UserName);
                _repository.Update(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<PeriodAuditResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PeriodAuditResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<PeriodAuditResponseDto>>> GetPaged(PeriodAuditFilterRequestDto paginationRequestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<PeriodAuditResponseDto>>();
            try
            {
                Expression<Func<PeriodAudit, bool>> filter = x => x.IsActive;
                if (!string.IsNullOrEmpty(paginationRequestDto.Filter))
                    filter = filter.AndAlso(x => x.GlobalObservations.Contains(paginationRequestDto.Filter) && x.IsActive);

                if (paginationRequestDto.StoreId.HasValue)
                    filter = filter.AndAlso(x => x.StoreId == paginationRequestDto.StoreId.Value && x.IsActive);

                if (paginationRequestDto.EnterpriseId.HasValue)
                    filter = filter.AndAlso(x => x.Store.EnterpriseId == paginationRequestDto.EnterpriseId.Value && x.IsActive);

                if (paginationRequestDto.ResponsibleAuditorId.HasValue)
                    filter = filter.AndAlso(x => x.ResponsibleAuditorId == paginationRequestDto.ResponsibleAuditorId.Value && x.IsActive);

                if (paginationRequestDto.EndDate.HasValue)
                    filter = filter.AndAlso(x => x.EndDate >= paginationRequestDto.EndDate.Value && x.IsActive);

                if (paginationRequestDto.StartDate.HasValue)
                    filter = filter.AndAlso(x => x.StartDate == paginationRequestDto.StartDate.Value && x.IsActive);

                Func<IQueryable<PeriodAudit>, IOrderedQueryable<PeriodAudit>> orderBy = q => q.OrderByDescending(x => x.CreationDate);

                var entities = await _repository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: paginationRequestDto.PageNumber,
                    pageSize: paginationRequestDto.PageSize,
                    includeProperties: [ x => x.Store.Enterprise, x => x.Administrator, x => x.Assistant, x => x.OperationManager, x => x.FloatingAdministrator, x => x.ResponsibleAuditor, x => x.AuditStatus, su => su.Supervisor]
                );

                var pagedResult = new PaginationResponseDto<PeriodAuditResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<PeriodAuditResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = paginationRequestDto.PageNumber,
                    PageSize = paginationRequestDto.PageSize
                };

                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PaginationResponseDto<PeriodAuditResponseDto>>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<bool>> Recalculate(Guid periodAuditId)
        {
            var response = ResponseDto.Create<bool>();
            try
            {
                var entity = await _repository.GetFirstOrDefaultAsync(x=>x.PeriodAuditId == periodAuditId && x.IsActive, includeProperties: [x=>x.Store]);
                if (entity == null)
                {
                    response = ResponseDto.Error<bool>("No se encontró el registro.");
                    return response;
                }
                var periodAuditGroupResults = await _periodAuditGroupResultRepository.GetByPeriodAuditIdAsync(periodAuditId);
                decimal acumulatedScore = 0;
                foreach (var groupResult in periodAuditGroupResults)
                {
                    var score = groupResult.ScoreValue * (groupResult.TotalWeighting / 100);
                    acumulatedScore += score;
                }
                var scaleCompany = await _scaleCompanyRepository.GetByEnterpriseIdAsync(entity.Store!.EnterpriseId);
                if (scaleCompany == null) {
                    response = ResponseDto.Error<bool>($"No se encontró la escala asociada a la empresa.");
                    return response;
                }

                bool scaleFound = false;
                foreach (var scale in scaleCompany)
                {
                    if (acumulatedScore >= scale.MinValue && acumulatedScore <= scale.MaxValue)
                    {
                        entity.ScaleName = scale.Name;
                        entity.ScaleIcon = scale.Icon;
                        entity.ScaleColor = scale.ColorCode;
                        scaleFound = true;
                        break;
                    }
                }
                if (!scaleFound)
                {
                    response = ResponseDto.Error<bool>("No se encontró una escala válida para la puntuación calculada.");
                    return response;
                }

                entity.ScoreValue = acumulatedScore;
                _repository.Update(entity);

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
