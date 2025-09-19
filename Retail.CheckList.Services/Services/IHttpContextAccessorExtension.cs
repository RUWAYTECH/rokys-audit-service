using Microsoft.AspNetCore.Http;
using Retail.CheckList.Common.Constant;
using Retail.CheckList.DTOs.Responses.User;

namespace Reatil.Services.Services
{
    public static class IHttpContextAccessorExtension
    {
        public static UserCurrentResponseDto CurrentUser(this IHttpContextAccessor httpContextAccessor)
        {
            var userName = httpContextAccessor?.HttpContext?.User?.FindFirst(Constants.ClaimNames.NameId)?.Value;
            var response = new UserCurrentResponseDto { UserName = userName };

            var nombreCompleto = httpContextAccessor?.HttpContext?.User?.FindFirst(Constants.ClaimNames.NombreCompleto)?.Value;
            response.NombreCompleto = nombreCompleto; 
            
            var applicationIdString = httpContextAccessor?.HttpContext?.User?.FindFirst(Constants.ClaimNames.ApplicationId)?.Value;
            int.TryParse(applicationIdString ?? "0", out int applicationId);
            response.ApplicationId = applicationId;

            var email = httpContextAccessor?.HttpContext?.User?.FindFirst(Constants.ClaimNames.Email)?.Value;
            response.Email = email;

            var profileIdString = httpContextAccessor?.HttpContext?.User?.FindFirst(Constants.ClaimNames.ProfileId)?.Value;
            int.TryParse(profileIdString ?? "0", out int profileId);
            response.ProfileId = profileId;

            var profileName = httpContextAccessor?.HttpContext?.User?.FindFirst(Constants.ClaimNames.ProfileName)?.Value;
            response.ProfileName = profileName;


            var employeeId = httpContextAccessor?.HttpContext?.User?.FindFirst(Constants.ClaimNames.EmployeeId)?.Value;
            int.TryParse(employeeId, out int outEmployeeId);
            response.EmployeeId = outEmployeeId;

          


            return response;
        }
    }
}

