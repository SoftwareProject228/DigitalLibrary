using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DigitalLibrary.Security.Models;

namespace DigitalLibrary.Security
{
	public static class JWTFactory
	{
		private static readonly Dictionary<User, List<string>> Tokens
			= new Dictionary<User, List<string>>();

		public static void AddToken(User user, string token, bool removePrev = false)
		{
			if (!Tokens.ContainsKey(user))
				Tokens[user] = new List<string>();

			if (removePrev)
				Tokens[user].Clear();

			Tokens[user].Add(token);
		}

		public static string GetLastTokenForJudge(User user)
		{
			if (!Tokens.ContainsKey(user)) return null;

			return Tokens[user].Last();
		}

		public static void RemoveToken(User user, string token)
		{
			if (!Tokens.ContainsKey(user)) return;
			Tokens[user].Remove(token);
		}

		public static IEnumerable<string> GetAllTokensForJudge(User user)
		{
			if (!Tokens.ContainsKey(user)) return new List<string>();

			return Tokens[user];
		}
    }
}