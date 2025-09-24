using Microsoft.AspNetCore.Http;
using Rokys.Audit.Common.Constant;
using Rokys.Audit.DTOs.Responses.User;

namespace Reatil.Services.Services
{
    public static class IHttpContextAccessorExtension
    {
        public static UserCurrentResponseDto CurrentUser(this IHttpContextAccessor httpContextAccessor)
        {
            var userName = httpContextAccessor?.HttpContext?.User?.FindFirst(Constants.ClaimNames.NameId)?.Value;
            var response = new UserCurrentResponseDto { UserName = userName };

            var first_name = httpContextAccessor?.HttpContext?.User?.FindFirst(Constants.ClaimNames.FirstName)?.Value;
            var last_name = httpContextAccessor?.HttpContext?.User?.FindFirst(Constants.ClaimNames.LastName)?.Value;

            response.FullName = $"{first_name} {last_name}";



            var email = httpContextAccessor?.HttpContext?.User?.FindFirst(Constants.ClaimNames.Email)?.Value;
            response.Email = email;



            var employeeId = httpContextAccessor?.HttpContext?.User?.FindFirst(Constants.ClaimNames.EmployeeId)?.Value;
            Guid.TryParse(employeeId, out Guid outEmployeeId);
            response.EmployeeId = outEmployeeId;


            var userId = httpContextAccessor?.HttpContext?.User?.FindFirst(Constants.ClaimNames.UserId)?.Value;
            Guid.TryParse(userId, out Guid outUserId);
            response.UserId = outUserId;




            return response;
        }
    }
}

