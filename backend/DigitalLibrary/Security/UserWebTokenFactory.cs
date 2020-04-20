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

		public static async Task<UserWebTokenValidation> CheckTokenValidationAsync(string token)
		{
			UserWebToken userToken;
			try
			{
				userToken = UserWebToken.FromToken(token);
			}
			catch
			{
				return null;
			}

			var userCollection = MongoConnection.GetCollection<User>("users");
			var foundUsers = await userCollection.FindAsync(new BsonDocument("_id", new ObjectId(userToken.UserId)));
			var foundUser = await foundUsers.FirstOrDefaultAsync();
			if (foundUser == null) return null;

			var validation = new UserWebTokenValidation
			{
				UserName = foundUser.UserName,
				UserStatus = foundUser.Status,
				ExpiresAt = userToken.ExpiresAt
			};

			var tokenFilterBuilder = Builders<AuthorizationToken>.Filter;
			var tokenFilter = tokenFilterBuilder.Eq("Token", userToken.Token);

			var tokenCollection = MongoConnection.GetCollection<AuthorizationToken>("authorization_tokens");
			var foundTokens = await tokenCollection.FindAsync(tokenFilter);
			if (foundTokens.ToList().Count == 0)
			{
				validation.Validation = UserWebToken.TokenValidation.Invalid;
				return validation;
			}

			if (userToken.ExpiresAt > DateTime.Now)
			{
				validation.Validation = UserWebToken.TokenValidation.Valid;
				return validation;
			}

			validation.Validation = UserWebToken.TokenValidation.Expired;
			return validation;
		}

		public static async Task DeleteTokenAsync(string token)
		{
			var tokenCollection = MongoConnection.GetCollection<AuthorizationToken>("authorization_tokens");
			await tokenCollection.DeleteOneAsync(new BsonDocument("Token", token));
		}
    }
}