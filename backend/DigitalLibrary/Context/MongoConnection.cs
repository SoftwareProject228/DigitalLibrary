using System.Collections.Generic;
using DigitalLibrary.Properties;
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
					_database = new MongoClient(Resource.MongoDBConnectionString)
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