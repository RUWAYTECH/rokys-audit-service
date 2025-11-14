using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Reatil.Services.Services;
using Rokys.Audit.DTOs.Requests.InboxItems;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.InboxItems;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class InboxItemsService : IInboxItemsService
    {
        private readonly IInboxItemsRepository _inboxRepository;
        private readonly IValidator<InboxItemRequestDto> _validator;
        private readonly ILogger<InboxItemsService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserReferenceRepository _userReferenceRepository;

        public InboxItemsService(
            IInboxItemsRepository inboxRepository,
            IValidator<InboxItemRequestDto> validator,
            ILogger<InboxItemsService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IUserReferenceRepository userReferenceRepository)
        {
            _inboxRepository = inboxRepository;
            _validator = validator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _userReferenceRepository = userReferenceRepository;
        }

        public async Task<ResponseDto<InboxItemResponseDto>> Create(InboxItemRequestDto requestDto)
        {
            var response = ResponseDto.Create<InboxItemResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }

                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<InboxItems>(requestDto);
                // resolve provided request user id (system user) to UserReferenceId if given
                entity.UserId = currentUser.UserReferenceId;

                // compute next sequence number for this PeriodAudit
                if (entity.PeriodAuditId.HasValue)
                {
                    var last = await _inboxRepository.GetFirstOrDefaultAsync(filter: x => x.PeriodAuditId == entity.PeriodAuditId.Value && x.IsActive, orderBy: q => q.OrderByDescending(x => x.SequenceNumber));
                    entity.SequenceNumber = (last?.SequenceNumber ?? 0) + 1;
                }

                if (!string.IsNullOrEmpty(requestDto.Action))
                    entity.Action = requestDto.Action;

                entity.CreateAudit(currentUser?.UserName ?? "system");
                _inboxRepository.Insert(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<InboxItemResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<InboxItemResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _inboxRepository.GetFirstOrDefaultAsync(filter: x => x.InboxItemId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error("No se encontró el item de inbox.");
                    return response;
                }
                entity.IsActive = false;
                _inboxRepository.Update(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<InboxItemResponseDto>>> GetPaged(InboxItemFilterRequestDto filterRequest)
        {
            var response = ResponseDto.Create<PaginationResponseDto<InboxItemResponseDto>>();
            try
            {
                Expression<Func<InboxItems, bool>> filter = x => x.IsActive;
                if (filterRequest.PeriodAuditId.HasValue)
                    filter = filter.AndAlso(x => x.PeriodAuditId == filterRequest.PeriodAuditId.Value && x.IsActive);

                if (filterRequest.NextStatusId.HasValue)
                    filter = filter.AndAlso(x => x.NextStatusId == filterRequest.NextStatusId.Value && x.IsActive);

                if (!string.IsNullOrEmpty(filterRequest.Filter))
                    filter = filter.AndAlso(x => x.Comments != null && x.Comments.Contains(filterRequest.Filter) && x.IsActive);

                Func<IQueryable<InboxItems>, IOrderedQueryable<InboxItems>> orderBy = q => q.OrderBy(x => x.SequenceNumber);

                var entities = await _inboxRepository.GetPagedAsync(filter: filter, orderBy: orderBy, pageNumber: filterRequest.PageNumber, pageSize: filterRequest.PageSize,

                includeProperties: [x => x.User, y => y.NextStatus, p => p.PrevStatus]);

                var pagedResult = new PaginationResponseDto<InboxItemResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<InboxItemResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = filterRequest.PageNumber,
                    PageSize = filterRequest.PageSize
                };
                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PaginationResponseDto<InboxItemResponseDto>>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<InboxItemResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<InboxItemResponseDto>();
            try
            {
                var entity = await _inboxRepository.GetFirstOrDefaultAsync(filter: x => x.InboxItemId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<InboxItemResponseDto>("No se encontró el item de inbox.");
                    return response;
                }
                response.Data = _mapper.Map<InboxItemResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<InboxItemResponseDto>(ex.Message);
            }
            return response;
        }
    }
}
