using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalLibrary.Model;
using DigitalLibrary.Security.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DigitalLibrary.Context
{
	public static class CatalogContext
	{
		private static IMongoCollection<CatalogNode> _collectionRaw;

		private static IMongoCollection<CatalogNode> _collectionPrimary;

		public static IMongoCollection<CatalogNode> CollectionRaw =>
			_collectionRaw ??= MongoConnection.GetCollection<CatalogNode>("raw_catalog");

		public static IMongoCollection<CatalogNode> CollectionPrimary =>
			_collectionPrimary ??= MongoConnection.GetCollection<CatalogNode>("primary_catalog");

		public static async Task AddNewPostAsync(WallPost post, User publisher)
		{
			var rawPost = new CatalogNode
			{
				Title = post.Title,
				ContentText = post.ContentText,
				Tags = post.Tags,
				CreationTime = DateTime.Now,
				Publisher = publisher,
				AttachedFiles = new List<AttachedFile>()
			};
			foreach (var fileId in post.AttachedFilesId)
			{
				var file = await FileContext.FindFileById(fileId);
				rawPost.AttachedFiles.Add(file);
			}

			rawPost.Status = CatalogNode.WallPostStatus.Reviewing;

			await CollectionRaw.InsertOneAsync(rawPost);
		}

		public static async Task AcceptPostAsync(string postId)
		{
			var result = await CollectionRaw.FindAsync(new BsonDocument("_id", new BsonObjectId(new ObjectId(postId))));
			var rawPost = await result.FirstOrDefaultAsync();
			rawPost.Status = CatalogNode.WallPostStatus.Accepted;
			await CollectionPrimary.InsertOneAsync(rawPost);
			await CollectionRaw.DeleteOneAsync(new BsonDocument("_id", new BsonObjectId(new ObjectId(postId))));
		}

		public static async Task DeclinePostAsync(string postId)
		{
			var setter = Builders<CatalogNode>.Update.Set("Status", CatalogNode.WallPostStatus.Declined);
			await CollectionRaw.UpdateOneAsync(new BsonDocument("_id", new BsonObjectId(new ObjectId(postId))), setter);
		}

		public static async Task DeletePostAsync(string postId)
		{
			var result = await CollectionPrimary.FindAsync(new BsonDocument("_id", new BsonObjectId(new ObjectId(postId))));
			var post = await result.FirstOrDefaultAsync();
			post.Status = CatalogNode.WallPostStatus.Deleted;
			await CollectionRaw.InsertOneAsync(post);
			await CollectionPrimary.DeleteOneAsync(new BsonDocument("_id", new BsonObjectId(new ObjectId(postId))));
		}

		public static async Task<CatalogNode> FindPostByIdRawAsync(string postId)
		{
			var filter = Builders<CatalogNode>.Filter.Eq("_id", new BsonObjectId(new ObjectId(postId))) &
			             Builders<CatalogNode>.Filter.Eq("Status", CatalogNode.WallPostStatus.Reviewing);
			var result = await CollectionRaw.FindAsync(filter);
			var rawPost = await result.FirstOrDefaultAsync();
			return rawPost;
		}

		public static async Task<CatalogNode> FindPostByIdPrimaryAsync(string postId)
		{
			var result = await CollectionPrimary.FindAsync(new BsonDocument("_id", new BsonObjectId(new ObjectId(postId))));
			var primaryPost = await result.FirstOrDefaultAsync();
			return primaryPost;
		}

		public static async Task<IEnumerable<CatalogNode>> FindPostsByTagRawAsync(string tag)
		{
			var filter = Builders<CatalogNode>.Filter.AnyEq("Tags", tag) &
			             Builders<CatalogNode>.Filter.Eq("Status", CatalogNode.WallPostStatus.Reviewing);
			var result = await CollectionRaw.FindAsync(filter);
			var posts = result.ToEnumerable().OrderByDescending(post => post.CreationTime);
			return posts;
		}

		public static async Task<IEnumerable<CatalogNode>> FindPostsByTagPrimaryAsync(string tag)
		{
			var filter = Builders<CatalogNode>.Filter.AnyEq("Tags", tag);
			var result = await CollectionPrimary.FindAsync(filter);
			var posts = result.ToEnumerable().OrderByDescending(post => post.CreationTime);
			return posts;
		}
	}
}