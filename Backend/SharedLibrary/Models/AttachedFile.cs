using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedLibrary.Models
{
	public class AttachedFile
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }
		public string FileName { get; set; }

		public long Weight { get; set; }

		public string LocalPath { get; set; }

		public string UserToken { get; set; }
	}
}