using Rokys.Audit.DTOs.Common;
using Rokys.Audit.DTOs.Requests.User;
using Rokys.Audit.DTOs.Responses.Common;
using Rokys.Audit.DTOs.Responses.User;
using Rokys.Audit.Services.Interfaces;

namespace Rokys.Audit.Services.Services
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
