using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DigitalLibrary.Client.Data
{
	public static class UserAuthorizationModel
	{
		public static string UserName { get; set; }

		public static string AuthenticationStatus { get; set; }

		public static string UserStatus { get; set; }

		public static string Token { get; set; }

		public static async Task AuthorizeAsync(string userName, string password)
		{
			var client = new HttpClient();
			client.DefaultRequestHeaders.Add("username", userName);
			client.DefaultRequestHeaders.Add("password", password);
			client.DefaultRequestHeaders.Add("application", "digitallibrary://web_app");
			var response = await client.GetAsync("https://localhost:44355/api/auth/login");
			var payload = JsonConvert.DeserializeObject<UserPayload>(await response.Content.ReadAsStringAsync());
			if (response.IsSuccessStatusCode)
			{
				UserName = payload.UserName;
				AuthenticationStatus = payload.AuthenticationStatus;
				UserStatus = payload.UserStatus;
				Token = payload.Token;
			}
			else
			{
				AuthenticationStatus = payload.AuthenticationStatus;
			}
		}
	}
}