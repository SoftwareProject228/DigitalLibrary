using DigitalLibrary.Security.Models;

namespace DigitalLibrary.Security
{
	public class RegistrationLinkPayload
	{

		public string UserName { get; set; }

		public string Email { get; set; }

		public string OwnerUserId { get; set; }

		public bool Available { get; set; }

		public User.UserStatus Status { get; set; }
	}
}