using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.Enterprise;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.Enterprise;
using Rokys.Audit.Globalization;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class EnterpriseService : IEnterpriseService
    {
        private readonly IEnterpriseRepository _enterpriseRepository;
        private readonly IValidator<EnterpriseRequestDto> _fluentValidator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEnterpriseThemeRepository _enterpriseThemeRepository;

        public EnterpriseService(
            IEnterpriseRepository enterpriseRepository,
            IValidator<EnterpriseRequestDto> fluentValidator,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            ILogger<EnterpriseService> logger,
            IHttpContextAccessor httpContextAccessor,
            IEnterpriseThemeRepository enterpriseThemeRepository)
        {
            _enterpriseRepository = enterpriseRepository;
            _fluentValidator = fluentValidator;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _enterpriseThemeRepository = enterpriseThemeRepository;
        }

        public async Task<ResponseDto<EnterpriseResponseDto>> Create(EnterpriseRequestDto requestDto)
        {
            var response = ResponseDto.Create<EnterpriseResponseDto>();
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
                    var entity = _mapper.Map<Enterprise>(requestDto);
                    entity.CreateAudit(currentUser.UserName);
                    _enterpriseRepository.Insert(entity);
                    await _unitOfWork.CommitAsync();
                    response.Data = _mapper.Map<EnterpriseResponseDto>(entity);
                }
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<EnterpriseResponseDto>(ex.Message);
                _logger.LogError(ex.Message);
            }
            return response;
        }

        public Task<ResponseDto> Delete(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<ResponseDto<EnterpriseResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<EnterpriseResponseDto>();
            try
            {
                var entity = await _enterpriseRepository.GetFirstOrDefaultAsync(filter: x => x.EnterpriseId == id && x.IsActive);
                response.Data = _mapper.Map<EnterpriseResponseDto>(entity);
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<EnterpriseResponseDto>(ex.Message);
                _logger.LogError(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<EnterpriseResponseDto>>> GetPaged(EnterpriseFilterRequestDto requestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<EnterpriseResponseDto>>();
            try
            {
                int totalRows;
                Expression<Func<Enterprise, bool>> filter = x => x.IsActive;

                if (!string.IsNullOrEmpty(requestDto.Filter))
                    filter = x => x.Name.Contains(requestDto.Filter);

                if (requestDto.StartDate.HasValue)
                    filter = filter.AndAlso(x => x.CreationDate >= requestDto.StartDate.Value);

                if (requestDto.EndDate.HasValue)
                    filter = filter.AndAlso(x => x.CreationDate <= requestDto.EndDate.Value);

                Func<IQueryable<Enterprise>, IOrderedQueryable<Enterprise>> orderBy = q => q.OrderBy(x => x.Name);

                var entities = await _enterpriseRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: requestDto.PageNumber,
                    pageSize: requestDto.PageSize
                );

                var pagedResult = new PaginationResponseDto<EnterpriseResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<EnterpriseResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = requestDto.PageNumber,
                    PageSize = requestDto.PageSize
                };

                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<PaginationResponseDto<EnterpriseResponseDto>>(ex.Message);
                _logger.LogError(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<EnterpriseResponseDto>> Update(Guid id, EnterpriseUpdateRequestDto requestDto)
        {
            var response = ResponseDto.Create<EnterpriseResponseDto>();
            try
            {
                var entity = _enterpriseRepository.GetByKey(id);
                if (entity == null)
                    response.Messages.Add(new ApplicationMessage { Message = ValidationMessage.NotFound, MessageType = ApplicationMessageType.Error });
                else
                {
                    var currentUser = _httpContextAccessor.CurrentUser();
                    _mapper.Map(requestDto, entity);
                    entity.UpdateAudit(currentUser.UserName);
                    _enterpriseRepository.Update(entity);

                    // Actualizar o crear theme si se envían propiedades de theme
                    await HandleEnterpriseThemeAsync(entity.EnterpriseId, requestDto, currentUser.UserName, isUpdate: true);

                    await _unitOfWork.CommitAsync();
                    response.Data = _mapper.Map<EnterpriseResponseDto>(entity);
                }
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<EnterpriseResponseDto>(ex.Message);
                _logger.LogError(ex.Message);
            }
            return response;
        }

        private async Task HandleEnterpriseThemeAsync(Guid enterpriseId, EnterpriseRequestDto requestDto, string userName, bool isUpdate = false)
        {
            var existingTheme = await _enterpriseThemeRepository.GetFirstOrDefaultAsync(t => t.EnterpriseId == enterpriseId);

            if (existingTheme != null)
            {
                // Actualizar theme existente
                existingTheme.PrimaryColor = requestDto.PrimaryColor ?? existingTheme.PrimaryColor;
                existingTheme.SecondaryColor = requestDto.SecondaryColor ?? existingTheme.SecondaryColor;
                existingTheme.AccentColor = requestDto.AccentColor ?? existingTheme.AccentColor;
                existingTheme.BackgroundColor = requestDto.BackgroundColor ?? existingTheme.BackgroundColor;
                existingTheme.TextColor = requestDto.TextColor ?? existingTheme.TextColor;

                if (requestDto.LogoData != null)
                {
                    existingTheme.LogoData = requestDto.LogoData;
                    existingTheme.LogoContentType = requestDto.LogoContentType;
                    existingTheme.LogoFileName = requestDto.LogoFileName;
                }

                existingTheme.UpdateAudit(userName);
                _enterpriseThemeRepository.Update(existingTheme);
            }
            else
            {
                // Crear nuevo theme
                var theme = new EnterpriseTheme
                {
                    EnterpriseId = enterpriseId,
                    PrimaryColor = requestDto.PrimaryColor ?? "#0066CC",
                    SecondaryColor = requestDto.SecondaryColor ?? "#333333",
                    AccentColor = requestDto.AccentColor,
                    BackgroundColor = requestDto.BackgroundColor,
                    TextColor = requestDto.TextColor,
                    LogoData = requestDto.LogoData,
                    LogoContentType = requestDto.LogoContentType,
                    LogoFileName = requestDto.LogoFileName
                };
                theme.CreateAudit(userName);
                _enterpriseThemeRepository.Insert(theme);
            }
        }
    }
}
