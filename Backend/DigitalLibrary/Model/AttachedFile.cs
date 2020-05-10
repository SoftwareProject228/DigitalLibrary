using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DigitalLibrary.Model
{
	public class AttachedFile
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public string FileName { get; set; }

		public long Lenght { get; set; }

		public string LocalPath { get; set; }

		[BsonRepresentation(BsonType.ObjectId)]
		public string UserId { get; set; }
	}
}