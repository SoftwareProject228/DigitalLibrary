using System;
using DigitalLibrary.Security.Models;

namespace DigitalLibrary.Security
{
	public class UserWebTokenValidation
	{
		public User User { get; set; }

		public DateTime ExpiresAt { get; set; }

		public UserWebToken.TokenValidation Validation { get; set; }

	}
}