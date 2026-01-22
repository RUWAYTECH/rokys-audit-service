using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Rokys.Audit.Common.Constant;
using Rokys.Audit.DTOs.Responses.User;
using Rokys.Audit.Infrastructure.Repositories;

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

            var userRepository = httpContextAccessor?.HttpContext?.RequestServices?.GetService<IUserReferenceRepository>();

            if (userRepository != null && outUserId != Guid.Empty)
            {
                try
                {
                    // Buscar información adicional del usuario en la base de datos
                    // Nota: Necesitarás verificar si tu modelo User usa Guid o long como ID
                    var userEntity = userRepository.GetFirstOrDefault(
                        u => u.UserId == outUserId && u.IsActive == true
                    );

                    if (userEntity != null)
                    {
                        response.UserReferenceId = userEntity.UserReferenceId;
                        response.RoleCodes = userEntity.RoleCode?.Split(',', StringSplitOptions.RemoveEmptyEntries)?.ToList() ?? [];
                    }
                }
                catch (Exception)
                {
                    throw new Exception("Error al obtener el usuario actual con datos del repositorio");
                }

            }


            return response;
        }
    }
}

