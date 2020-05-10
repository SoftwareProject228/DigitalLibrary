using DigitalLibrary.Authentication;

namespace DigitalLibrary.Model
{
	public class UserAuthenticationResponse
	{
		public string UserName { get; set; }

		public string Email { get; set; }

		public UserRole UserRole { get; set; }

		public string Token { get; set; }
	}
}