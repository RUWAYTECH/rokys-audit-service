using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.CriteriaSubResult;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.CriteriaSubResult;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Reatil.Services.Services;

namespace Rokys.Audit.Services.Services
{
	public class CriteriaSubResultService : ICriteriaSubResultService
	{
		private readonly IRepository<CriteriaSubResult> _criteriaSubResultRepository;
		private readonly IValidator<CriteriaSubResultRequestDto> _fluentValidator;
		private readonly ILogger<CriteriaSubResultService> _logger;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IAMapper _mapper;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public CriteriaSubResultService(
			IRepository<CriteriaSubResult> criteriaSubResultRepository,
			IValidator<CriteriaSubResultRequestDto> fluentValidator,
			ILogger<CriteriaSubResultService> logger,
			IUnitOfWork unitOfWork,
			IAMapper mapper,
			IHttpContextAccessor httpContextAccessor)
		{
			_criteriaSubResultRepository = criteriaSubResultRepository;
			_fluentValidator = fluentValidator;
			_logger = logger;
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_httpContextAccessor = httpContextAccessor;
		}

		public async Task<ResponseDto<PaginationResponseDto<CriteriaSubResultResponseDto>>> GetPaged(PaginationRequestDto paginationRequestDto)
		{
			var response = ResponseDto.Create<PaginationResponseDto<CriteriaSubResultResponseDto>>();
			try
			{
				Expression<Func<CriteriaSubResult, bool>> filter = x => x.IsActive;
				if (!string.IsNullOrEmpty(paginationRequestDto.Filter))
					filter = x => x.CriteriaName.Contains(paginationRequestDto.Filter) && x.IsActive;

				Func<IQueryable<CriteriaSubResult>, IOrderedQueryable<CriteriaSubResult>> orderBy = q => q.OrderByDescending(x => x.CreationDate);

				var entities = await _criteriaSubResultRepository.GetPagedAsync(
					filter: filter,
					orderBy: orderBy,
					pageNumber: paginationRequestDto.PageNumber,
					pageSize: paginationRequestDto.PageSize
				);

				var pagedResult = new PaginationResponseDto<CriteriaSubResultResponseDto>
				{
					Items = _mapper.Map<IEnumerable<CriteriaSubResultResponseDto>>(entities.Items),
					TotalCount = entities.TotalRows,
					PageNumber = paginationRequestDto.PageNumber,
					PageSize = paginationRequestDto.PageSize
				};

				response.Data = pagedResult;
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				response = ResponseDto.Error<PaginationResponseDto<CriteriaSubResultResponseDto>>(ex.Message);
			}
			return response;
		}

		public async Task<ResponseDto<CriteriaSubResultResponseDto>> Create(CriteriaSubResultRequestDto requestDto)
		{
			var response = ResponseDto.Create<CriteriaSubResultResponseDto>();
			try
			{
				var validate = _fluentValidator.Validate(requestDto);
				if (!validate.IsValid)
				{
					response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
					return response;
				}
				var currentUser = _httpContextAccessor.CurrentUser();
				var entity = _mapper.Map<CriteriaSubResult>(requestDto);
				entity.CreateAudit(currentUser.UserName);
				_criteriaSubResultRepository.Insert(entity);
				await _unitOfWork.CommitAsync();
				response.Data = _mapper.Map<CriteriaSubResultResponseDto>(entity);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				response = ResponseDto.Error<CriteriaSubResultResponseDto>(ex.Message);
			}
			return response;
		}

		public async Task<ResponseDto<CriteriaSubResultResponseDto>> GetById(Guid id)
		{
			var response = ResponseDto.Create<CriteriaSubResultResponseDto>();
			try
			{
				var entity = await _criteriaSubResultRepository.GetFirstOrDefaultAsync(filter: x => x.CriteriaSubResultId == id && x.IsActive);
				if (entity == null)
				{
					response = ResponseDto.Error<CriteriaSubResultResponseDto>("No se encontró el subcriterio.");
					return response;
				}
				response.Data = _mapper.Map<CriteriaSubResultResponseDto>(entity);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				response = ResponseDto.Error<CriteriaSubResultResponseDto>(ex.Message);
			}
			return response;
		}

		public async Task<ResponseDto<CriteriaSubResultResponseDto>> Update(Guid id, CriteriaSubResultRequestDto requestDto)
		{
			var response = ResponseDto.Create<CriteriaSubResultResponseDto>();
			try
			{
				var validate = _fluentValidator.Validate(requestDto);
				if (!validate.IsValid)
				{
					response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
					return response;
				}
				var entity = await _criteriaSubResultRepository.GetFirstOrDefaultAsync(filter: x => x.CriteriaSubResultId == id && x.IsActive);
				if (entity == null)
				{
					response = ResponseDto.Error<CriteriaSubResultResponseDto>("No se encontró el subcriterio.");
					return response;
				}
				var currentUser = _httpContextAccessor.CurrentUser();
				entity = _mapper.Map(requestDto, entity);
				entity.UpdateAudit(currentUser.UserName);
				_criteriaSubResultRepository.Update(entity);
				await _unitOfWork.CommitAsync();
				response.Data = _mapper.Map<CriteriaSubResultResponseDto>(entity);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				response = ResponseDto.Error<CriteriaSubResultResponseDto>(ex.Message);
			}
			return response;
		}

		public async Task<ResponseDto> Delete(Guid id)
		{
			var response = ResponseDto.Create();
			try
			{
				var entity = await _criteriaSubResultRepository.GetFirstOrDefaultAsync(filter: x => x.CriteriaSubResultId == id && x.IsActive);
				if (entity == null)
				{
					response = ResponseDto.Error("No se encontró el subcriterio.");
					return response;
				}
				entity.IsActive = false;
				_criteriaSubResultRepository.Update(entity);
				await _unitOfWork.CommitAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.Message);
				response = ResponseDto.Error(ex.Message);
			}
			return response;
		}
	}
}