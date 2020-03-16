using System;

namespace DigitalLibrary.Security
{
	public class JWTHead
	{
		public string Host { get; set; }

		public string Algorithm { get; set; }

		public DateTime ExpiresAt { get; set; }
	}
}