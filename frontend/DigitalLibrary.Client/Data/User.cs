using System.Collections.Generic;

namespace DigitalLibrary.Client.Data
{
	public class User
	{
		public string Id { get; set; }

		public string UserName { get; set; }

		public UserRole Status { get; set; }

		public string Email { get; set; }

		public List<string> AssociatedTags { get; set; }

		public string RegisteredByLink { get; set; }
	}
}
