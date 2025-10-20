using Rokys.Audit.DTOs.Requests.StorageFiles;
using Rokys.Audit.DTOs.Responses.StorageFiles;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Common;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IStorageFilesService : IBaseService<StorageFileRequestDto, StorageFileResponseDto>
    {
        Task<ResponseDto<PaginationResponseDto<StorageFileListResponseDto>>> GetPaged(StorageFileFilterRequestDto requestDto);
    }
}
