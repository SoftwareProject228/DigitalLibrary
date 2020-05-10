using DigitalLibrary.Authentication;

namespace DigitalLibrary.Model
{
	public class RegistrationResponse
	{
		public string UserName { get; set; }

		public UserRole UserStatus { get; set; }

		public string Email { get; set; }
	}
}