using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.DTOs.Requests.Proveedor;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.Proveedor;
using Rokys.Audit.Globalization;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.IQuery;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using Rokys.Audit.Services.Validations;
using System.Linq.Expressions;

namespace Rokys.Audit.Services.Services
{
    public class ProveedorService : IProveedorService
    {
        private readonly IProveedorRepository _proveedorRepository;
        private readonly IValidator<ProveedorRequestDto> _fluentValidator;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly ILogger _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IProveedorQuery _proveedorQuery;
        public ProveedorService(IHttpContextAccessor httpContextAccessor, IProveedorRepository proveedorRepository,
         IValidator<ProveedorRequestDto> fluentValidator
            , IUnitOfWork unitOfWork, IAMapper mapper, ILogger<ProveedorService> logger, IProveedorQuery proveedorQuery)
        {
            _proveedorRepository = proveedorRepository;
            _fluentValidator = fluentValidator;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _proveedorQuery= proveedorQuery;
        }

        public async Task<ResponseDto<ProveedorResponseDto>> Create(ProveedorRequestDto requestDto)
        {
            var response = ResponseDto.Create<ProveedorResponseDto>();

            try
            {
                var validate = _fluentValidator.Validate(requestDto);

                if (!validate.IsValid)
                    response.Messages.AddRange(validate.ToResponse().Messages);
                else
                {
                    var currentUser = _httpContextAccessor.CurrentUser();
                    var entity = _mapper.Map<Proveedor>(requestDto);
                    entity.CreateAudit(currentUser.UserName);
                    _proveedorRepository.Insert(entity);
                    await _unitOfWork.CommitAsync();

                    response.Data = _mapper.Map<ProveedorResponseDto>(entity);
                }
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<ProveedorResponseDto>(ex.Message);
                _logger.LogError(ex.Message);
            }

            return response;
        }

        public async Task<ResponseDto> Delete(object id)
        {
            var response = ResponseDto.Create();

            try
            {
                var entity = _proveedorRepository.GetByKey(id);

                if (entity == null)
                    response.Messages.Add(new ApplicationMessage { Message = ValidationMessage.NotFound, MessageType = ApplicationMessageType.Error });
                else
                {
                    _proveedorRepository.Delete(entity);
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

        public async Task<ResponseDto<IEnumerable<ProveedorResponseDto>>> Get(object filter)
        {
            var response = ResponseDto.Create<IEnumerable<ProveedorResponseDto>>();

            try
            {
                var requestFilter = (ProveedorRequestDto)filter;

                /*Expression<Func<Proveedor, bool>> queryFilter =
                    x => (x.RUC == (string.IsNullOrEmpty(requestFilter.RUC) ? x.RUC : requestFilter.RUC))
                    && (x.RazonSocial.Contains(requestFilter.RazonSocial));

                var entities = await _proveedorRepository.GetAsync(queryFilter);    */

                var entities = await _proveedorQuery.GetAllAsync();
                response.Data = _mapper.Map<IEnumerable<ProveedorResponseDto>>(entities);
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<IEnumerable<ProveedorResponseDto>>(ex.Message);
                _logger.LogError(ex.Message);
            }

            return response;
        }

        public async Task<ResponseDto<ProveedorResponseDto>> GetById(object id)
        {
            var response = ResponseDto.Create<ProveedorResponseDto>();

            try
            {
                var entity = _proveedorRepository.GetByKey(id);
                response.Data = _mapper.Map<ProveedorResponseDto>(entity);
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<ProveedorResponseDto>(ex.Message);
                _logger.LogError(ex.Message);
            }

            return response;
        }

        public async Task<ResponseDto<ProveedorResponseDto>> GetByRuc(string ruc)
        {
            var response = ResponseDto.Create<ProveedorResponseDto>();

            try
            {
                if (string.IsNullOrEmpty(ruc))
                {
                    response.WithMessage("El RUC no puede ser vació", messageType: ApplicationMessageType.Error);
                    return response;
                }
                    
                if (ruc.Length!=11)
                {
                    response.WithMessage("El RUC debe contener 11 caracteres", messageType: ApplicationMessageType.Error);
                    return response;
                }
                   
                response.Data = _mapper.Map<ProveedorResponseDto>(new Proveedor { IdProveedor = 1, RUC = ruc, RazonSocial= $"RAZON SOCIAL MOCK {ruc}" });
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<ProveedorResponseDto>(ex.Message);
                _logger.LogError(ex.Message);
            }

            return response;
        }

        public async Task<ResponseDto<ProveedorResponseDto>> Update(object idProveedor, ProveedorRequestDto requestDto)
        {
            var response = ResponseDto.Create<ProveedorResponseDto>();

            try
            {
                var entity = _proveedorRepository.GetByKey(idProveedor);

                if (entity == null)
                    response.Messages.Add(new ApplicationMessage { Message = ValidationMessage.NotFound, MessageType = ApplicationMessageType.Error });
                else
                {
                    var currentUser = _httpContextAccessor.CurrentUser();
                    _mapper.Map(requestDto, entity);
                    entity.UpdateAudit(currentUser.UserName);
                    _proveedorRepository.Update(entity);
                    await _unitOfWork.CommitAsync();

                    response.Data = _mapper.Map<ProveedorResponseDto>(entity);
                }
            }
            catch (Exception ex)
            {
                response = ResponseDto.Error<ProveedorResponseDto>(ex.Message);
                _logger.LogError(ex.Message);
            }

            return response;
        }
    }
}
