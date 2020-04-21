using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DigitalLibrary.Security.Models
{
	public class User
	{
		public static class UserStatus
		{
			public static string Student { get; } = "student";
			public static string Professor { get; } = "professor";
			public static string Moderator { get; } = "moderator";
		}

		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public string UserName { get; set; }

		public string PasswordHash { get; set; }

		public string Status { get; set; }

		public string Email { get; set; }

		public List<string> AssociatedTags { get; set; }

		[BsonRepresentation(BsonType.ObjectId)]
		public string RegisteredByLink { get; set; }
	}
}
