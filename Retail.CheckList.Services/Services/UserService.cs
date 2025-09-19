using Retail.CheckList.DTOs.Common;
using Retail.CheckList.DTOs.Requests.User;
using Retail.CheckList.DTOs.Responses.Common;
using Retail.CheckList.DTOs.Responses.User;
using Retail.CheckList.Services.Interfaces;

namespace Retail.CheckList.Services.Services
{
    public class UserService: IUserService
    {
    
        public UserService() {
        }

        public async Task<ResponseDto<UserResponseDto>> Login(LoginRequestDto loginRequestDto)
        {
            var result = new ResponseDto<UserResponseDto>(new UserResponseDto
            {
                UserName = "Cristian"
            });
          
            var response =  ResponseDto.Create<UserResponseDto>(result?.Data);
            response.Messages.AddRange(result.Messages);
            return response;
        }

    }
}
