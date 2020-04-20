using System;
using System.Threading.Tasks;
using DigitalLibrary.Context;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DigitalLibrary.Security
{
	public class RegistrationLinkWebTokenFactory
	{
		public static async Task AddTokenAsync(RegistrationLinkWebToken token, string ownerUserToken)
		{
			var tokenCollection = MongoConnection.GetCollection<RegistrationToken>("registration_links");

			var authToken = new RegistrationToken
			{
				Available = true,
				LinkToken = token.Token,
				OwnerUserToken = ownerUserToken
			};
			await tokenCollection.InsertOneAsync(authToken);
		}

		public static async Task<bool> CheckAvailabilityAsync(string token)
		{
			var tokenCollection = MongoConnection.GetCollection<RegistrationToken>("registration_links");
			var result = await tokenCollection.FindAsync(new BsonDocument("LinkToken", token));
			var link = await result.FirstOrDefaultAsync();
			if (link == null || !link.Available) return false;
			try
			{
				var registrationToken = RegistrationLinkWebToken.FromToken(token);
				if (registrationToken.ExpiresAt <= DateTime.Now) return false;
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static async Task InvalidateLinkAsync(string token)
		{
			var tokenCollection = MongoConnection.GetCollection<RegistrationToken>("registration_links");
			var update = Builders<RegistrationToken>.Update.Set("Available", false);
			await tokenCollection.UpdateOneAsync(new BsonDocument("LinkToken", token), update);
		}
	}
}