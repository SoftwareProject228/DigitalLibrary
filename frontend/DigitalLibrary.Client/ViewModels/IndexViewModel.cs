using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using DigitalLibrary.Client.Annotations;
using Microsoft.AspNetCore.ProtectedBrowserStorage;

namespace DigitalLibrary.Client.ViewModels
{
	public class IndexViewModel : INotifyPropertyChanged
	{

		public IndexViewModel()
		{
			Components = new ObservableCollection<ILayoutItemViewModel> {new CommandInputViewModel()};
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