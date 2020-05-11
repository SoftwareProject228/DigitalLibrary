using System.ComponentModel;
using System.Runtime.CompilerServices;
using DigitalLibrary.Client.Annotations;

namespace DigitalLibrary.Client.ViewModels
{
	public class CommandInputViewModel : ILayoutItemViewModel, INotifyPropertyChanged
	{
		public ItemType Type { get; } = ItemType.CommandItem;

		public string Command { get; set; }

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
