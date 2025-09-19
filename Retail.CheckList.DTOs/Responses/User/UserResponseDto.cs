namespace Retail.CheckList.DTOs.Responses.User
{
	public class UserResponseDto
	{
		public string UserToken { get; set; }
		public double Expiration { get; set; }
		public string ProfileFirstName { get; set; }	
        public string UserName { get; set; }
    }
}

