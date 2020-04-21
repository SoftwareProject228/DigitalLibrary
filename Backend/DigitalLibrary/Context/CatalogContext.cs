using DigitalLibrary.Model;
using MongoDB.Driver;

namespace DigitalLibrary.Context
{
	public static class CatalogContext
	{
		private static IMongoCollection<CatalogNode> _collectionRaw;

		private static IMongoCollection<CatalogNode> _collectionPrimary;

		public static IMongoCollection<CatalogNode> Collection =>
			_collectionRaw ??= MongoConnection.GetCollection<CatalogNode>("raw_catalog");
	}
}