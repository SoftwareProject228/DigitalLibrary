using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DigitalLibrary.Security.Models
{
	public class AuthorizationToken
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		[BsonRepresentation(BsonType.ObjectId)]
		public string UserId { get; set; }

		public DateTime ExpiresAt { get; set; }

		public string Token { get; set; }

		public string Application { get; set; }
	}
}
