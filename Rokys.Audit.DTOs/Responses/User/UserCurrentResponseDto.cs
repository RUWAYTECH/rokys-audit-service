using Rokys.Audit.Common.Constant;

namespace Rokys.Audit.DTOs.Responses.User
{
	public class UserCurrentResponseDto
	{
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }

        [Obsolete("This property is deprecated and will be removed in future versions.")]
        public int ProfileId { get; set; }
        [Obsolete("This property is deprecated and will be removed in future versions.")]
        public string ProfileName { get; set; }
        [Obsolete("This property is deprecated and will be removed in future versions.")]
        public int ApplicationId { get; set; }
        public Guid EmployeeId { get; set; }
        public string Email { get; set; }
        public string Position { get; set; }

        public bool IsSupervisor
        {
            get
            {
                return ProfileName == RoleCode.Supervisor.ToString();
            }
        }

        public bool IsAdministrator
        {
            get
            {
                return ProfileName == RoleCode.Administrador.ToString();
            }
        }

        public bool IsEmployee
        {
            get
            {
                return ProfileName == RoleCode.Empleado.ToString();
            }
        }

        public bool IsRRHH
        {
            get
            {
                return ProfileName == RoleCode.RRHH.ToString();
            }
        }
    }
}

