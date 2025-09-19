namespace Retail.CheckList.DTOs.Responses.User
{
	public class UserCurrentResponseDto
	{
		public string UserName { get; set; }
        public string NombreCompleto { get; set; }
        public int ProfileId { get; set; }
		public string ProfileName { get; set; }
		public int ApplicationId { get; set; }
		public int EmployeeId { get; set; }
		public string Email { get; set; }
		public string CodigoDivision { get; set; }
	}
}

