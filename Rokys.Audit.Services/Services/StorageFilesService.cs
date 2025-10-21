using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.StorageFiles;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.StorageFiles;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using System.Linq.Expressions;
using static Rokys.Audit.Common.Constant.Constants;

namespace Rokys.Audit.Services.Services
{
    public class StorageFilesService : IStorageFilesService
    {
        private readonly IStorageFilesRepository _storageFilesRepository;
        private readonly IValidator<StorageFileRequestDto> _validator;
        private readonly ILogger<StorageFilesService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly FileSettings _fileSettings;

        public StorageFilesService(
            IStorageFilesRepository storageFilesRepository,
            IValidator<StorageFileRequestDto> validator,
            ILogger<StorageFilesService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            FileSettings fileSettings)
        {
            _storageFilesRepository = storageFilesRepository;
            _validator = validator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _fileSettings = fileSettings;
        }

        public async Task<ResponseDto<StorageFileResponseDto>> Create(StorageFileRequestDto requestDto)
        {
            var response = ResponseDto.Create<StorageFileResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<StorageFiles>(requestDto);
                if (requestDto.File != null && requestDto.File.Length > 0)
                {
                    var (originalName, filePath) = await SaveMemoFileAsync(requestDto.File);
                    entity.OriginalName = requestDto.File.FileName;
                    entity.FileName = originalName;
                    entity.FileUrl = filePath;
                    entity.EntityId = requestDto.EntityId;
                    entity.FileType = Path.GetExtension(requestDto.File.FileName).ToLower();
                    entity.EntityName = requestDto.EntityName;
                    entity.ClassificationType = requestDto.ClassificationType;
                    entity.UploadDate = DateTime.Now;
                    entity.UploadedBy = currentUser.UserName;
                }
                entity.CreateAudit(currentUser.UserName);
                _storageFilesRepository.Insert(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<StorageFileResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<StorageFileResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _storageFilesRepository.GetFirstOrDefaultAsync(filter: x => x.StorageFileId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error("No se encontró el registro.");
                    return response;
                }
                entity.IsActive = false;
                entity.UpdateDate = DateTime.UtcNow;
                _storageFilesRepository.Update(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<StorageFileResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<StorageFileResponseDto>();
            try
            {
                var entity = await _storageFilesRepository.GetFirstOrDefaultAsync(filter: x => x.StorageFileId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<StorageFileResponseDto>("No se encontró el archivo.");
                    return response;
                }
                response.Data = _mapper.Map<StorageFileResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<StorageFileResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<StorageFileListResponseDto>>> GetPaged(StorageFileFilterRequestDto requestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<StorageFileListResponseDto>>();
            try
            {
                Expression<Func<StorageFiles, bool>> filter = x => x.IsActive;
                if (requestDto.EntityId.HasValue)
                    filter = filter.AndAlso(x => x.EntityId == requestDto.EntityId.Value);
                if (!string.IsNullOrEmpty(requestDto.FileName))
                    filter = filter.AndAlso(x => x.FileName.Contains(requestDto.FileName));

                Func<IQueryable<StorageFiles>, IOrderedQueryable<StorageFiles>> orderBy = q => q.OrderByDescending(x => x.CreationDate);

                var entities = await _storageFilesRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: requestDto.PageNumber,
                    pageSize: requestDto.PageSize
                );

                var pagedResult = new PaginationResponseDto<StorageFileListResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<StorageFileListResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = requestDto.PageNumber,
                    PageSize = requestDto.PageSize
                };

                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PaginationResponseDto<StorageFileListResponseDto>>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<StorageFileResponseDto>> Update(Guid id, StorageFileRequestDto requestDto)
        {
            var response = ResponseDto.Create<StorageFileResponseDto>();
            try
            {
                var validate = _validator.Validate(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }
                var entity = await _storageFilesRepository.GetFirstOrDefaultAsync(filter: x => x.StorageFileId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error<StorageFileResponseDto>("No se encontró el registro.");
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                entity = _mapper.Map(requestDto, entity);
                entity.UpdateAudit(currentUser.UserName);
                _storageFilesRepository.Update(entity);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<StorageFileResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<StorageFileResponseDto>(ex.Message);
            }
            return response;
        }
        private async Task<(string fileName, string filePath)> SaveMemoFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return (null, null);
            var uploadsFolder = Path.Combine(_fileSettings.Path, FileDirectories.Uploads);
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);
            var fileName = $"{Guid.NewGuid()}_{file.FileName}";
            var filePath = Path.Combine(uploadsFolder, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return (file.FileName, fileName);
        }
    }
}
