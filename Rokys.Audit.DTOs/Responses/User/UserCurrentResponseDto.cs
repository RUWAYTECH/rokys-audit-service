using Rokys.Audit.Common.Constant;

namespace Rokys.Audit.DTOs.Responses.User
{
	public class UserCurrentResponseDto
	{
        public Guid UserId { get; set; }
        public Guid UserReferenceId { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public Guid EmployeeId { get; set; }
        public string Email { get; set; }
           
        public List<string> RoleCodes { get; set; } = new List<string>();

    }
}

