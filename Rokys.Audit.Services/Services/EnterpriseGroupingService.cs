using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.EnterpriseGrouping;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.Enterprise;
using Rokys.Audit.DTOs.Responses.EnterpriseGrouping;
using Rokys.Audit.DTOs.Responses.Store;
using Rokys.Audit.Globalization;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class EnterpriseGroupingService : IEnterpriseGroupingService
    {
        private readonly IValidator<EnterpriseGroupingRequestDto> _fluentValidator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEnterpriseGroupingRepository _enterpriseGroupingRepository;
        private readonly IEnterpriseGroupRepository _enterpriseGroupRepository;

        public EnterpriseGroupingService(
            IValidator<EnterpriseGroupingRequestDto> fluentValidator,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            ILogger<EnterpriseGroupingService> logger,
            IHttpContextAccessor httpContextAccessor,
            IEnterpriseGroupingRepository enterpriseGroupingRepository,
            IEnterpriseGroupRepository enterterpriseGroupRepository
            )
        {
            _fluentValidator = fluentValidator;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _enterpriseGroupingRepository = enterpriseGroupingRepository;
            _enterpriseGroupRepository = enterterpriseGroupRepository;
        }

        public async Task<ResponseDto<EnterpriseGroupingResponseDto>> Create(EnterpriseGroupingCreateRequestDto requestDto)
        {
            var response = ResponseDto.Create<EnterpriseGroupingResponseDto>();
            try
            {
                var validate = _fluentValidator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                else
                {
                    var currentUser = _httpContextAccessor.CurrentUser();
                    var entity = _mapper.Map<EnterpriseGrouping>(requestDto);

                    entity.CreateAudit(currentUser.UserName);
                    _enterpriseGroupingRepository.Insert(entity);

                    if (requestDto.EnterpriseIds.Count() > 0)
                    {
                        var duplicatedIds = requestDto.EnterpriseIds
                            .GroupBy(x => x)
                            .Where(g => g.Count() > 1)
                            .Select(g => g.Key)
                            .ToList();

                        if (duplicatedIds.Any())
                        {
                            response.Messages.Add(new ApplicationMessage
                            {
                                Message = "Existen empresas duplicadas en la solicitud.",
                                MessageType = ApplicationMessageType.Error
                            });
                            return response;
                        }

                        var existingEnterpriseIds = await _enterpriseGroupRepository.ExistsEnterpriseInOtherGroupAsync(requestDto.EnterpriseIds, entity.EnterpriseGroupingId);

                        if (existingEnterpriseIds)
                        {
                            response.Messages.Add(new ApplicationMessage
                            {
                                Message = "Una o más empresas ya pertenecen a otro grupo.",
                                MessageType = ApplicationMessageType.Error
                            });
                            return response;
                        }


                        foreach (var enterpriseId in requestDto.EnterpriseIds)
                        {
                            var enterpriseGroup = new EnterpriseGroup
                            {
                                EnterpriseGroupingId = entity.EnterpriseGroupingId,
                                EnterpriseId = enterpriseId
                            };
                            enterpriseGroup.CreateAudit(currentUser.UserName);
                            _enterpriseGroupRepository.Insert(enterpriseGroup);
                        }
                    }

                    await _unitOfWork.CommitAsync();
                    response.Data = _mapper.Map<EnterpriseGroupingResponseDto>(entity);
                }
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<EnterpriseGroupingResponseDto>(ex.Message);
                _logger.LogError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = _enterpriseGroupingRepository.GetByKey(id);
                Guid enterpriseGroupingId = entity.EnterpriseGroupingId;
                var getEnterpriseGroups = await _enterpriseGroupRepository.GetByEnterpriseGroupingId(id);
                if (entity == null)
                    response.Messages.Add(new ApplicationMessage { Message = ValidationMessage.NotFound, MessageType = ApplicationMessageType.Error });
                else
                {
                    if (getEnterpriseGroups.Count() > 0)
                    {
                        foreach (var enterpriseGroup in getEnterpriseGroups)
                        {
                            _enterpriseGroupRepository.Delete(enterpriseGroup);
                        }
                    }
                    _enterpriseGroupingRepository.Delete(entity);
                    await _unitOfWork.CommitAsync();
                }
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error(ex.Message);
                _logger.LogError(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<EnterpriseGroupingResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<EnterpriseGroupingResponseDto>();
            try
            {
                var entity = await _enterpriseGroupingRepository.GetByEnterpriseGroupingId(id);
                if (entity == null)
                    response.Messages.Add(new ApplicationMessage { Message = ValidationMessage.NotFound, MessageType = ApplicationMessageType.Error });
                else
                    response.Data = _mapper.Map<EnterpriseGroupingResponseDto>(entity);
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<EnterpriseGroupingResponseDto>(ex.Message);
                _logger.LogError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseDto<List<EnterpriseGroupingResponseDto>>> GetList()
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto<PaginationResponseDto<EnterpriseGroupingResponseDto>>> GetPaged(EnterpriseGroupingFilterRequestDto requestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<EnterpriseGroupingResponseDto>>();
            try
            {
                Expression<Func<EnterpriseGrouping, bool>> filter = x => x.IsActive;

                Func<IQueryable<EnterpriseGrouping>, IOrderedQueryable<EnterpriseGrouping>> orderBy = q => q.OrderBy(x => x.Name);

                if (!string.IsNullOrEmpty(requestDto.Filter))
                    filter = filter.AndAlso(x => x.Name.Contains(requestDto.Filter) || x.Description.Contains(requestDto.Filter) || x.Code.Contains(requestDto.Filter));


                var result = await _enterpriseGroupingRepository.GetPagedCustomAsync(
                    filter,
                    orderBy,
                    requestDto.PageNumber,
                    requestDto.PageSize
                );
                response.Data = new PaginationResponseDto<EnterpriseGroupingResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<EnterpriseGroupingResponseDto>>(result.Items),
                    TotalCount = result.TotalRows,
                    PageNumber = requestDto.PageNumber,
                    PageSize = requestDto.PageSize
                };
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<PaginationResponseDto<EnterpriseGroupingResponseDto>>(ex.Message);
                _logger.LogError(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<EnterpriseGroupingResponseDto>> Update(Guid id, EnterpriseGroupingRequestDto dto)
        {
            var response = ResponseDto.Create<EnterpriseGroupingResponseDto>();
            try
            {
                var validate = _fluentValidator.Validate(dto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                else
                {
                    var entity = await _enterpriseGroupingRepository.GetByKeyAsync(id);
                    if (entity == null)
                    {
                        response.Messages.Add(new ApplicationMessage { Message = ValidationMessage.NotFound, MessageType = ApplicationMessageType.Error });
                        return response;
                    }
                    var currentUser = _httpContextAccessor.CurrentUser();
                    _mapper.Map(dto, entity);
                    entity.UpdateAudit(currentUser.UserName);
                    _enterpriseGroupingRepository.Update(entity);
                    await _unitOfWork.CommitAsync();
                    var newEntity = await _enterpriseGroupingRepository.GetFirstOrDefaultAsync(filter: eg => eg.EnterpriseGroupingId == id, includeProperties: x => x.EnterpriseGroups);
                    response.Data = _mapper.Map<EnterpriseGroupingResponseDto>(newEntity);
                }
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<EnterpriseGroupingResponseDto>(ex.Message);
                _logger.LogError(ex.Message);
            }
            return response;
        }
        public async Task<ResponseDto> DeleteEnterpriseGroupById(Guid Id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _enterpriseGroupRepository.GetByKeyAsync(Id);
                if (entity == null)
                {
                    response.Messages.Add(new ApplicationMessage { Message = ValidationMessage.NotFound, MessageType = ApplicationMessageType.Error });
                    return response;
                }
                _enterpriseGroupRepository.Delete(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error(ex.Message);
                _logger.LogError(ex.Message);
            }
            return response;
        }

    }
}
