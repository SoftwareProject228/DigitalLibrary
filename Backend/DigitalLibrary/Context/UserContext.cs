using System.Threading.Tasks;
using DigitalLibrary.Security.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DigitalLibrary.Context
{
	public static class UserContext
	{
		private static IMongoCollection<User> _userCollection;

		public static IMongoCollection<User> Collection =>
			_userCollection ??= MongoConnection.GetCollection<User>("users");

		public static async Task<User> FindUserByNameAsync(string userName)
		{
			var result = await Collection.FindAsync(new BsonDocument("UserName", userName));
			var user = await result.FirstOrDefaultAsync();
			return user;
		}

		public static async Task<string> AddUserAsync(User user)
		{
			await Collection.InsertOneAsync(user);
			return user.Id;
		}

		public static async Task<User> FindUserById(string id)
		{
			var result = await Collection.FindAsync(new BsonDocument("_id", id));
			var user = await result.FirstOrDefaultAsync();
			return user;
		}
	}
}