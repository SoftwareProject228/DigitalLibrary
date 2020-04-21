using System.Threading.Tasks;
using DigitalLibrary.Model;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DigitalLibrary.Context
{
	public static class FileContext
	{
		private static IMongoCollection<AttachedFile> _collection;

		public static IMongoCollection<AttachedFile> Collection =>
			_collection ??= MongoConnection.GetCollection<AttachedFile>("files");

		public static async Task<string> AddFileAsync(AttachedFile file)
		{
			await Collection.InsertOneAsync(file);
			return file.Id;
		}

		public static async Task<AttachedFile> FindFileById(string id)
		{
			var result = await Collection.FindAsync(new BsonDocument("_id", new BsonObjectId(new ObjectId(id))));
			var file = await result.FirstOrDefaultAsync();
			return file;
		}
	}
}