using System;
using DigitalLibrary.Security.Models;

namespace DigitalLibrary.Security
{
	public class UserWebTokenValidation
	{
		public string UserName { get; set; }

		public DateTime ExpiresAt { get; set; }

		public UserWebToken.TokenValidation Validation { get; set; }

		public User.UserStatus UserStatus { get; set; }
	}
}