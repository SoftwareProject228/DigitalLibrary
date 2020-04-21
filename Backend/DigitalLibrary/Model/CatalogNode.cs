using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DigitalLibrary.Model
{
	public class CatalogNode
	{
		public enum WallPostStatus
		{
			Accepted,
			Declined,
			Reviewing,
			Deleted
		}

		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public DateTime CreationTime { get; set; }

		public string Title { get; set; }

		public string ContentText { get; set; }

		public List<string> Tags { get; set; }

		[BsonRepresentation(BsonType.ObjectId)]
		public string UserId { get; set; }

		public WallPostStatus Status { get; set; }

		public List<AttachedFile> AttachedFiles { get; set; }
	}
}