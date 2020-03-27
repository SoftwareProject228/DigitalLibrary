using System.Collections.Generic;
using MongoDB.Driver;

namespace DigitalLibrary.Context
{
	public static class MongoConnection
	{
		private static IMongoDatabase _database;

		private static Dictionary<string, object> _collections;

		public static IMongoDatabase Database
		{
			get
			{
				if (_database == null)
					_database = new MongoClient(
							@"mongodb+srv://DigitalLibrary:23081975@mongospace-1jtjh.azure.mongodb.net/test?retryWrites=true&w=majority")
						.GetDatabase("digital_library_storage");
				_collections = new Dictionary<string, object>();
				return _database;
			}
		}

		public static IMongoCollection<T> GetCollection<T>(string name)
		{
			if (Database != null && !_collections.ContainsKey(name))
				_collections[name] = _database.GetCollection<T>(name);
			return (IMongoCollection<T>) _collections[name];
		}
	}
}