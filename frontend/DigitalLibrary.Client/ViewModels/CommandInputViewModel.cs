using System.ComponentModel;
using System.Runtime.CompilerServices;
using DigitalLibrary.Client.Annotations;

namespace DigitalLibrary.Client.ViewModels
{
	public class CommandInputViewModel : ILayoutItemViewModel, INotifyPropertyChanged
	{
		public ItemType Type { get; } = ItemType.CommandItem;

		private string _command;
		private string _userName;

		public string Command
		{
			get => _command;
			set
			{
				if (_command == value) return;
				_command = value;
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

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
