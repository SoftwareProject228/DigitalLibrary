using System.Collections.Generic;
using DigitalLibrary.Authentication;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DigitalLibrary.Security.Models
{
	public class User
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public string UserName { get; set; }

		public string PasswordHash { get; set; }

		public UserRole Status { get; set; }

		public string Email { get; set; }

		public List<string> AssociatedTags { get; set; }

		[BsonRepresentation(BsonType.ObjectId)]
		public string RegisteredByLink { get; set; }
	}
}
