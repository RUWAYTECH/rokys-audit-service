using Retail.CheckList.DTOs.Requests.User;
using Retail.CheckList.DTOs.Responses.Common;
using Retail.CheckList.DTOs.Responses.User;

namespace Retail.CheckList.Services.Interfaces
{
    public interface IUserService
    {
        Task<ResponseDto<UserResponseDto>> Login(LoginRequestDto loginRequestDto);

    }
}
