using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using DigitalLibrary.Client.Annotations;
using DigitalLibrary.Client.Data;
using Newtonsoft.Json;

namespace DigitalLibrary.Client.ViewModels
{
	public class SearchViewModel : INotifyPropertyChanged, ILayoutItemViewModel
	{
		public SearchViewModel(string token, PostsContext context, Action<string, Dictionary<string, string>> onFinish, Dictionary<string, string> initParameters)
		{
			Context = context;
			_token = token;
			_onFinish = onFinish;
			ExecuteCommand("search", initParameters);
		}

		private bool _isClosed;
		private readonly string _token;
		private string _userName;
		private CatalogNode _currentlyOpened;
		private readonly Action<string, Dictionary<string, string>> _onFinish;
		public PostsContext Context { get; }

		public CatalogNode CurrentlyOpened
		{
			get => _currentlyOpened;
			set
			{
				if (_currentlyOpened == value) return;
				_currentlyOpened = value;
				OnPropertyChanged();
			}
		}

		public bool IsClosed
		{
			get => _isClosed;
			set
			{
				if (_isClosed == value) return;
				_isClosed = value;
				OnPropertyChanged();
			}
		}

		public string UserName
		{
			get => _userName;
			set
			{
				if (_userName == value) return;
				_userName = value;
				OnPropertyChanged();
			}
		}

		public async void ExecuteCommand(string command, Dictionary<string, string> parameters)
		{
			if (command == "search")
			{
				var url = $"https://localhost:44355/api/wall/search?token={_token}";
				foreach (var (key, value) in parameters)
					url += "&" + key.Trim('-') + "=" + value;
				var client = new HttpClient();
				var response = await client.GetAsync(url);
				if (response.IsSuccessStatusCode)
				{
					var posts = JsonConvert.DeserializeObject<List<CatalogNode>>(
						await response.Content.ReadAsStringAsync());
					Context.SetPosts(posts);
					CurrentlyOpened = null;
				}
			}
			else if (command == "open")
			{
				var id = int.Parse(parameters["-id"]);
				CurrentlyOpened = Context.Posts.FirstOrDefault(elem => elem.Key == id).Value;
			}
			else
			{
				IsClosed = true;
				_onFinish(command, parameters);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public ItemType Type { get; } = ItemType.CommandSearch;
	}
}