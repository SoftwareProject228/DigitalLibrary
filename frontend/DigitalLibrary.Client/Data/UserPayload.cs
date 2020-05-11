namespace DigitalLibrary.Client.Data
{
	public class UserPayload
	{
		public string UserName { get; set; }

		public string Token { get; set; }

		public string Email { get; set; }

		public UserRole UserRole { get; set; }
	}
}