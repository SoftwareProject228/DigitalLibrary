using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DigitalLibrary.Context;
using DigitalLibrary.Security.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DigitalLibrary.Security
{
	public static class UserWebTokenFactory
	{
		public static async Task AddTokenAsync(UserWebToken token)
		{
			var tokenCollection = MongoConnection.GetCollection<AuthorizationToken>("authorization_tokens");

			var authToken = new AuthorizationToken
			{
				ExpiresAt = token.ExpiresAt,
				UserId = token.UserId,
				Token = token.Token,
				Application = token.Application
			};

			var builder = Builders<AuthorizationToken>.Filter;
			var filter = builder.Eq("UserId", authToken.UserId) & builder.Eq("Application", authToken.Application);

			await tokenCollection.DeleteManyAsync(filter);
			await tokenCollection.InsertOneAsync(authToken);
		}

		public static async Task<IEnumerable<AuthorizationToken>> GetAllTokensForUserAsync(string userId)
		{
			var tokenCollection = MongoConnection.GetCollection<AuthorizationToken>("authorization_tokens");

			var filter = new BsonDocument("UserId", userId);

			return (await tokenCollection.FindAsync(filter)).ToList();
		}

		public static async Task<UserWebToken.TokenValidation> CheckTokenValidation(string token)
		{
			UserWebToken userToken;
			try
			{
				userToken = UserWebToken.FromToken(token);
			}
			catch
			{
				return UserWebToken.TokenValidation.Invalid;
			}

			var tokenFilterBuilder = Builders<AuthorizationToken>.Filter;
			var tokenFilter = tokenFilterBuilder.Eq("Token", userToken.Token);

			var tokenCollection = MongoConnection.GetCollection<AuthorizationToken>("authorization_tokens");
			var foundTokens = await tokenCollection.FindAsync(tokenFilter);
			if (foundTokens.ToList().Count == 0) return UserWebToken.TokenValidation.Invalid;

			if (userToken.ExpiresAt > DateTime.Now) return UserWebToken.TokenValidation.Valid;
			return UserWebToken.TokenValidation.Expired;
		}
    }
}