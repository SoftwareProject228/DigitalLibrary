using System;
using DigitalLibrary.Authentication;

namespace DigitalLibrary.Model
{
	public class RegistrationLinkResponse
	{
		public string LinkToken { get; set; }

		public UserRole ForUserStatus { get; set; }

		public string ForUserName { get; set; }

		public string ForEmail { get; set; }

		public DateTime ExpiresAt { get; set; }

		public bool Available { get; set; }

	}
}