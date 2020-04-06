using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DigitalLibrary.Security.Models
{
	public class User
	{
		public enum UserStatus
		{
			Student,
			Moderator,
			Professor
		}

		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public string UserName { get; set; }

		public string PasswordHash { get; set; }

		public UserStatus Status { get; set; }

		public string Email { get; set; }

		public List<string> AssociatedTags { get; set; }
	}
}
