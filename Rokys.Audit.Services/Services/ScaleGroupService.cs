using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Reatil.Services.Services;
using Rokys.Audit.Common.Extensions;
using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.ScaleGroup;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.ScaleGroup;
using Rokys.Audit.Infrastructure.IMapping;
using Rokys.Audit.Infrastructure.Persistence.Abstract;
using Rokys.Audit.Infrastructure.Repositories;
using Rokys.Audit.Model.Tables;
using Rokys.Audit.Services.Interfaces;
using Rokys.Audit.Services.Validations;
using System.Linq.Expressions;
using static Rokys.Audit.Common.Constant.Constants;

namespace Rokys.Audit.Services.Services
{
    public class ScaleGroupService : IScaleGroupService
    {
        private readonly IScaleGroupRepository _scaleGroupRepository;
        private readonly IValidator<ScaleGroupRequestDto> _fluentValidator;
        private readonly ILogger<ScaleGroupService> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly FileSettings _fileSettings;
        private readonly IStorageFilesRepository _storageFilesRepository;

        public ScaleGroupService(
            IScaleGroupRepository scaleGroupRepository,
            IValidator<ScaleGroupRequestDto> fluentValidator,
            ILogger<ScaleGroupService> logger,
            IUnitOfWork unitOfWork,
            IAMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            FileSettings fileSettings,
            IStorageFilesRepository storageFilesRepository)
        {
            _scaleGroupRepository = scaleGroupRepository;
            _fluentValidator = fluentValidator;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _fileSettings = fileSettings;
            _storageFilesRepository = storageFilesRepository;
        }

        public async Task<ResponseDto<ScaleGroupResponseDto>> Create(ScaleGroupRequestDto requestDto)
        {
            var response = ResponseDto.Create<ScaleGroupResponseDto>();
            try
            {
                var validate = await _fluentValidator.ValidateAsync(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage { Message = e.ErrorMessage, MessageType = ApplicationMessageType.Error }));
                    return response;
                }

                // Verificar si ya existe un código duplicado
                var existsByCode = await _scaleGroupRepository.ExistsByCodeAsync(requestDto.Code);
                if (existsByCode)
                {
                    response.Messages.Add(new ApplicationMessage { Message = "Ya existe un grupo de escala con este código.", MessageType = ApplicationMessageType.Error });
                    return response;
                }
                var currentUser = _httpContextAccessor.CurrentUser();
                var entity = _mapper.Map<ScaleGroup>(requestDto);
                entity.CreateAudit(currentUser.UserName);
                _scaleGroupRepository.Insert(entity);
                var storageFile = new StorageFiles();
                if (requestDto.HasSourceData == true && requestDto.File == null)
                {
                    response.Messages.Add(new ApplicationMessage
                    {
                        Message = "Si seleccionó que es con archivo, debe subir un archivo.",
                        MessageType = ApplicationMessageType.Error
                    });
                    return response;
                }
                if (requestDto.File != null && requestDto.File.Length > 0)
                {
                    requestDto.HasSourceData = true;
                    var (originalName, filePath) = await SaveMemoFileAsync(requestDto.File);
                    storageFile.OriginalName = requestDto.File.FileName;
                    storageFile.FileName = originalName;
                    storageFile.FileUrl = filePath;
                    storageFile.EntityId = entity.ScaleGroupId;
                    storageFile.FileType = Path.GetExtension(requestDto.File.FileName).ToLower();
                    storageFile.EntityName = entity.Name;
                    storageFile.ClassificationType = "Data source template";
                    storageFile.UploadDate = DateTime.Now;
                    storageFile.UploadedBy = currentUser.UserName;
                }
                storageFile.CreateAudit(currentUser.UserName);
                _storageFilesRepository.Insert(storageFile);
                await _unitOfWork.CommitAsync();
                response.Data = _mapper.Map<ScaleGroupResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<ScaleGroupResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto> Delete(Guid id)
        {
            var response = ResponseDto.Create();
            try
            {
                var entity = await _scaleGroupRepository.GetFirstOrDefaultAsync(filter: x => x.ScaleGroupId == id && x.IsActive);
                if (entity == null)
                {
                    response = ResponseDto.Error("No se encontró el grupo de escala.");
                    return response;
                }
                entity.IsActive = false;
                var currentUser = _httpContextAccessor.CurrentUser();
                entity.UpdateAudit(currentUser.UserName);
                _scaleGroupRepository.Update(entity);
                await _unitOfWork.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<PaginationResponseDto<ScaleGroupResponseDto>>> GetPaged(ScaleGroupFilterRequestDto paginationRequestDto)
        {
            var response = ResponseDto.Create<PaginationResponseDto<ScaleGroupResponseDto>>();
            try
            {
                Expression<Func<ScaleGroup, bool>> filter = x => x.IsActive;

                Func<IQueryable<ScaleGroup>, IOrderedQueryable<ScaleGroup>> orderBy = q => q.OrderByDescending(x => x.CreationDate);

                if (!string.IsNullOrEmpty(paginationRequestDto.Filter))
                    filter = filter.AndAlso(x => x.Code.Contains(paginationRequestDto.Filter) || x.Name.Contains(paginationRequestDto.Filter));

                if (paginationRequestDto.GroupId.HasValue)
                    filter = filter.AndAlso(x => x.GroupId == paginationRequestDto.GroupId.Value);

                var entities = await _scaleGroupRepository.GetPagedAsync(
                    filter: filter,
                    orderBy: orderBy,
                    pageNumber: paginationRequestDto.PageNumber,
                    pageSize: paginationRequestDto.PageSize,
                    includeProperties: [a => a.Group]
                );

                var pagedResult = new PaginationResponseDto<ScaleGroupResponseDto>
                {
                    Items = _mapper.Map<IEnumerable<ScaleGroupResponseDto>>(entities.Items),
                    TotalCount = entities.TotalRows,
                    PageNumber = paginationRequestDto.PageNumber,
                    PageSize = paginationRequestDto.PageSize
                };

                response.Data = pagedResult;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<PaginationResponseDto<ScaleGroupResponseDto>>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<ScaleGroupResponseDto>> GetById(Guid id)
        {
            var response = ResponseDto.Create<ScaleGroupResponseDto>();
            try
            {
                var entity = await _scaleGroupRepository.GetFirstOrDefaultAsync(
                    filter: x => x.ScaleGroupId == id && x.IsActive,
                   includeProperties: [a => a.Group]
                );
                var storageFiles = await _storageFilesRepository.GetFirstOrDefaultAsync(
                    filter: x => x.EntityId == id && x.IsActive
                );
                if (entity == null)
                {
                    response = ResponseDto.Error<ScaleGroupResponseDto>("No se encontró el grupo de escala.");
                    return response;
                }
                var mapData = _mapper.Map<ScaleGroupResponseDto>(entity);
                if (storageFiles != null)
                {
                    mapData.StorageFileName = storageFiles?.FileName;
                    mapData.SotrageFileId = storageFiles?.StorageFileId;
                }
                    
                response.Data = mapData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<ScaleGroupResponseDto>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<IEnumerable<ScaleGroupResponseDto>>> GetByGroupId(Guid groupId)
        {
            var response = ResponseDto.Create<IEnumerable<ScaleGroupResponseDto>>();
            try
            {
                var entities = await _scaleGroupRepository.GetAsync(
                    filter: x => x.GroupId == groupId && x.IsActive,
                    orderBy: q => q.OrderBy(x => x.Name),
                    includeProperties: [a => a.Group]
                );
                response.Data = _mapper.Map<IEnumerable<ScaleGroupResponseDto>>(entities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                response = ResponseDto.Error<IEnumerable<ScaleGroupResponseDto>>(ex.Message);
            }
            return response;
        }

        public async Task<ResponseDto<ScaleGroupResponseDto>> Update(Guid id, ScaleGroupRequestDto requestDto)
        {
            var response = ResponseDto.Create<ScaleGroupResponseDto>();

            try
            {
                var validator = new ScaleGroupValidator(_scaleGroupRepository, id);
                var validate = await validator.ValidateAsync(requestDto);
                if (!validate.IsValid)
                {
                    response.Messages.AddRange(validate.Errors.Select(e => new ApplicationMessage
                    {
                        Message = e.ErrorMessage,
                        MessageType = ApplicationMessageType.Error
                    }));
                    return response;
                }

                var entity = await _scaleGroupRepository.GetFirstOrDefaultAsync(x => x.ScaleGroupId == id && x.IsActive);
                if (entity == null)
                    return ResponseDto.Error<ScaleGroupResponseDto>("No se encontró el grupo de escala.");

                if (await _scaleGroupRepository.ExistsByCodeAsync(requestDto.Code, id))
                {
                    response.Messages.Add(new ApplicationMessage
                    {
                        Message = "Ya existe un grupo de escala con este código.",
                        MessageType = ApplicationMessageType.Error
                    });
                    return response;
                }

                if (requestDto.HasSourceData == true && requestDto.File == null)
                {
                    response.Messages.Add(new ApplicationMessage
                    {
                        Message = "Si seleccionó que es con archivo, debe subir un archivo.",
                        MessageType = ApplicationMessageType.Error
                    });
                    return response;
                }

                var currentUser = _httpContextAccessor.CurrentUser();
                
                if (requestDto.File?.Length > 0)
                    await SaveOrReplaceFileAsync(id, entity, requestDto, currentUser.UserName);

                _mapper.Map(requestDto, entity);
                entity.UpdateAudit(currentUser.UserName);
                _scaleGroupRepository.Update(entity);
                await _unitOfWork.CommitAsync();

                response.Data = _mapper.Map<ScaleGroupResponseDto>(entity);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                response = ResponseDto.Error<ScaleGroupResponseDto>(ex.Message);
            }

            return response;
        }
        private async Task SaveOrReplaceFileAsync(Guid entityId, ScaleGroup entity, ScaleGroupRequestDto requestDto, string userName)
        {
            var existingFile = await _storageFilesRepository.GetFirstOrDefaultAsync(x => x.EntityId == entityId && x.IsActive);

            if (existingFile != null)
                _storageFilesRepository.Delete(existingFile);

            var (originalName, filePath) = await SaveMemoFileAsync(requestDto.File);
            var newFile = new StorageFiles
            {
                OriginalName = requestDto.File.FileName,
                FileName = filePath,
                FileUrl = filePath,
                EntityId = entity.ScaleGroupId,
                FileType = Path.GetExtension(requestDto.File.FileName).ToLower(),
                EntityName = entity.Name,
                ClassificationType = "Data source template",
                UploadDate = DateTime.Now,
                UploadedBy = userName
            };
            newFile.CreateAudit(userName);
            _storageFilesRepository.Insert(newFile);

            requestDto.HasSourceData = true;
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