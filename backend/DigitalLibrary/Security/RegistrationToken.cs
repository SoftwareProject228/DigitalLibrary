using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DigitalLibrary.Security
{
	public class RegistrationToken
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public string OwnerUserToken { get; set; }

		public string LinkToken { get; set; }

		public bool Available { get; set; }
	}
}