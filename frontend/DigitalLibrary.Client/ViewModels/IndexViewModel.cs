using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DigitalLibrary.Client.Annotations;
using DigitalLibrary.Client.Data;

namespace DigitalLibrary.Client.ViewModels
{
	public class IndexViewModel : INotifyPropertyChanged
	{
		private readonly UserAuthorizationViewModel _userViewModel;
		private string _userName;
		private string _token;

		public IndexViewModel(UserAuthorizationViewModel userViewModel)
		{
			_userViewModel = userViewModel;
			UserName = _userViewModel.UserName;
			_userViewModel.PropertyChanged -= UserViewModelOnPropertyChanged;
			_userViewModel.PropertyChanged += UserViewModelOnPropertyChanged;
			Components = new ObservableCollection<ILayoutItemViewModel>();
		}

		public void InitializeComponents()
		{
			Components.Add(new CommandInputViewModel {UserName = UserName});
		}

		public void ExecuteCommand(string command, IDictionary<string, string> parameters)
		{
			var commandText = command;
			if (parameters != null)
			{
				foreach (var (key, value) in parameters)
				{
					commandText += ' ' + key + ' ' + value;
				}
			}
			if (Components[^1].Type == ItemType.CommandItem)
				Components.RemoveAt(Components.Count - 1);
			Components.Add(new CommandStompViewModel {CommandText = commandText, UserName = UserName});
			if (command == "search")
				Components.Add(new SearchViewModel(_token, new PostsContext(), OnSearchFinished,  new Dictionary<string, string>(parameters)));
			else if (command == "uploadpost")
				Components.Add(new UploadPostViewModel(Token, OnUploadPostFinish));
			else if (command == "library")
				Components.Add(new LibraryViewModel(_token, new PostsContext(), OnSearchFinished));
			else
				Components.Add(new CommandInputViewModel {UserName = UserName});
		}

		private void OnUploadPostFinish()
		{
			Components.Add(new CommandInputViewModel {UserName = UserName});
		}

		private void OnSearchFinished(string command, Dictionary<string, string> parameters)
		{
			ExecuteCommand(command, parameters);
		}

		private void UserViewModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == nameof(UserAuthorizationViewModel.UserName))
			{
				UserName = _userViewModel.UserName;
			}
			else if (e.PropertyName == nameof(UserAuthorizationViewModel.Token))
			{
				Token = _userViewModel.Token;
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

		public string UserName
		{
			get => _userName;
			private set
			{
				if (_userName == value) return;
				_userName = value;
				OnPropertyChanged();
			}
		}

		public ObservableCollection<ILayoutItemViewModel> Components { get; set; }
		
		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}