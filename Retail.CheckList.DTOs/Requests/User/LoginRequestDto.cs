using System;
namespace Rokys.Audit.DTOs.Requests.User
{
	public class LoginRequestDto
	{
		public string UserName { get; set; }
		public string Password { get; set; }
		public string ApplicationCode { get; set; }
	}
}

