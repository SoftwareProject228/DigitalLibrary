using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLibrary.Models
{
	public class WallPost
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }
		public DateTime CreationTime { get; set; }

		public string Title { get; set; }

		public string ContentText { get; set; }

		public List<string> Tags { get; set; }

		public string UserToken { get; set; }

		public List<AttachedFile> AttachedFiles { get; set; }
	}
}