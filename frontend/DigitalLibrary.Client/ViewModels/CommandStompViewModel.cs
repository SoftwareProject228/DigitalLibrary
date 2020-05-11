using System.ComponentModel;
using System.Runtime.CompilerServices;
using DigitalLibrary.Client.Annotations;

namespace DigitalLibrary.Client.ViewModels
{
	public class CommandStompViewModel : ILayoutItemViewModel, INotifyPropertyChanged
	{
		public string CommandText { get; set; }

		public ItemType Type { get; } = ItemType.CommandStomp;

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
