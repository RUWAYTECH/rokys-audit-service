using Rokys.Audit.DTOs.Requests.User;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.User;

namespace Rokys.Audit.Services.Interfaces
{
    public interface IUserService
    {
        Task<ResponseDto<UserResponseDto>> Login(LoginRequestDto loginRequestDto);

    }
}
