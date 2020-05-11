using System;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using DigitalLibrary.Client.Annotations;
using DigitalLibrary.Client.Data;
using Microsoft.AspNetCore.ProtectedBrowserStorage;
using Newtonsoft.Json;

namespace DigitalLibrary.Client.ViewModels
{
	public class UserAuthorizationViewModel : INotifyPropertyChanged
	{
		public UserAuthorizationViewModel(ProtectedLocalStorage storage)
		{
			_storage = storage;
		}

		private readonly ProtectedLocalStorage _storage;
		private string _userName;
		private string _token;
		private UserRole _userRole;

		public string UserName
		{
			get => _userName;
			set
			{
				if (value == _userName) return;
				_userName = value;
				OnPropertyChanged();
			}
		}

		public UserRole UserRole
		{
			get => _userRole;
			set
			{
				if (_userRole == value) return;
				_userRole = value;
				OnPropertyChanged();
			}
		}

		public string Token
		{
			get => _token;
			set
			{
				if (_token == value) return;
				_token = value;
				OnPropertyChanged();
			}
		}

		public async Task AuthorizeAsync(string userName, string password)
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
				UserRole = payload.UserRole;
				Token = payload.Token;
				await _storage.SetAsync("token", Token);
			}
		}

		public async Task ClearAuthorizationAsync()
		{
			UserName = null;
			Token = null;
			await _storage.DeleteAsync("token");
		}

		public async Task<bool> RestoreAuthorization()
		{
			var token = await _storage.GetAsync<string>("token");
			if (String.IsNullOrWhiteSpace(token))
				return false;
			var client = new HttpClient();
			var response = await client.GetAsync($"https://localhost:44355/api/auth/getUser?token={token}");
			if (!response.IsSuccessStatusCode)
			{
				await _storage.DeleteAsync("token");
				return false;
			}

			var result = JsonConvert.DeserializeObject<UserPayload>(await response.Content.ReadAsStringAsync());
			Token = result.Token;
			UserName = result.UserName;
			UserRole = result.UserRole;
			return true;
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}