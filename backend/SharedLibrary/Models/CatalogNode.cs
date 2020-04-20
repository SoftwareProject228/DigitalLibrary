using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLibrary.Models
{
	class CatalogNode
	{
		public enum NodeStatus
		{
			Accepted,
			Declined,
			Reviewing,
			Deleted
		}

		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public NodeStatus Status { get; set; }

		[BsonRepresentation(BsonType.ObjectId)]
		public string WallPost { get; set; }
	}
}
