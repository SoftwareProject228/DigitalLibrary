using System.Threading.Tasks;
using DigitalLibrary.Security.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DigitalLibrary.Context
{
	public static class RegistrationLinkContext
	{
		private static IMongoCollection<RegistrationLink> _linkCollection;

		public static IMongoCollection<RegistrationLink> Collection => _linkCollection ??=
			MongoConnection.GetCollection<RegistrationLink>("registration_links");

		public static async Task<RegistrationLink> FindLinkByIdAsync(string id)
		{
			var result = await Collection.FindAsync(new BsonDocument("_id", new BsonObjectId(new ObjectId(id))));
			var link = await result.FirstOrDefaultAsync();
			return link;
		}

		public static async Task<string> AddLinkAsync(RegistrationLink link)
		{
			await Collection.InsertOneAsync(link);
			return link.Id;
		}

		public static async Task InvalidateLinkAsync(string id)
		{
			var update = Builders<RegistrationLink>.Update.Set("Available", false);
			await Collection.UpdateOneAsync(new BsonDocument("_id", new BsonObjectId(new ObjectId(id))), update);
		}
	}
}