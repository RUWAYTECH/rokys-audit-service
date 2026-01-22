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
        private readonly IStoreRepository _storeRepository;

        public EnterpriseService(
            IEnterpriseRepository enterpriseRepository,
            IValidator<EnterpriseRequestDto> fluentValidator,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            ILogger<EnterpriseService> logger,
            IHttpContextAccessor httpContextAccessor,
            IEnterpriseThemeRepository enterpriseThemeRepository,
            IStoreRepository storeRepository)
        {
            _enterpriseRepository = enterpriseRepository;
            _fluentValidator = fluentValidator;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _enterpriseThemeRepository = enterpriseThemeRepository;
            _storeRepository = storeRepository;
        }

        public async Task<ResponseDto<EnterpriseResponseDto>> Create(EnterpriseCreateRequestDto requestDto)
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
                    if (requestDto.EnterpriseId != null)
                    {
                        entity.EnterpriseId = requestDto.EnterpriseId.Value;
                    }
                    
                    entity.CreateAudit(currentUser.UserName);
                    _enterpriseRepository.Insert(entity);
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

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = _enterpriseRepository.GetByKey(id);
                Guid enterpriseId = entity.EnterpriseId;
                var getStoreByEnterpriseId = await _storeRepository.GetFirstOrDefaultAsync(x => x.EnterpriseId == enterpriseId && x.IsActive);
                if (getStoreByEnterpriseId != null)
                {
                    response.Messages.Add(new ApplicationMessage { Message = "No se puede eliminar esta empresa porque tiene tiendas relacionadas.", MessageType = ApplicationMessageType.Error });
                    return response;
                }
                if (entity == null)
                    response.Messages.Add(new ApplicationMessage { Message = ValidationMessage.NotFound, MessageType = ApplicationMessageType.Error });
                else
                {
                    _enterpriseRepository.Delete(entity);
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

        public async Task<ResponseDto<EnterpriseResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<EnterpriseResponseDto>();
            try
            {
                var entity = await _enterpriseRepository.GetFirstOrDefaultAsync(filter: x => x.EnterpriseId == id && x.IsActive, includeProperties: e=>e.Theme);
                if (entity == null)
                {
                    response.Messages.Add(new ApplicationMessage
                    {
                        Message = "Enterprise not found",
                        MessageType = ApplicationMessageType.Error
                    });
                    return response;
                }

                var dto = _mapper.Map<EnterpriseResponseDto>(entity);

                if (entity.Theme != null)
                {
                    dto.PrimaryColor = entity.Theme.PrimaryColor;
                    dto.SecondaryColor = entity.Theme.SecondaryColor;
                    dto.AccentColor = entity.Theme.AccentColor;
                    dto.BackgroundColor = entity.Theme.BackgroundColor;
                    dto.TextColor = entity.Theme.TextColor;

                    dto.LogoData = entity.Theme.LogoData;
                    dto.LogoContentType = entity.Theme.LogoContentType;
                    dto.LogoFileName = entity.Theme.LogoFileName;
                }
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
                    pageSize: requestDto.PageSize,
                    includeProperties: e => e.Theme
                );

                var items = _mapper.Map<List<EnterpriseResponseDto>>(entities.Items);

                foreach (var item in items)
                {
                    var entity = entities.Items.First(e => e.EnterpriseId == item.EnterpriseId);

                    if (entity.Theme != null)
                    {
                        item.PrimaryColor = entity.Theme.PrimaryColor;
                        item.SecondaryColor = entity.Theme.SecondaryColor;
                        item.AccentColor = entity.Theme.AccentColor;
                        item.BackgroundColor = entity.Theme.BackgroundColor;
                        item.TextColor = entity.Theme.TextColor;

                        item.LogoData = entity.Theme.LogoData;
                        item.LogoContentType = entity.Theme.LogoContentType;
                        item.LogoFileName = entity.Theme.LogoFileName;
                    }
                }

                var pagedResult = new PaginationResponseDto<EnterpriseResponseDto>
                {
                    Items = items,
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
                var entity = await _enterpriseRepository.GetByEnterpriseId(id);
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
            var existingTheme = await _enterpriseThemeRepository.GetByEnterpriseId(enterpriseId);

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
