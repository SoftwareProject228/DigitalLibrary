using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using DigitalLibrary.Client.Annotations;

namespace DigitalLibrary.Client.Data
{
	public class PostsContext : INotifyPropertyChanged
	{
		public PostsContext()
		{
			Posts = new ObservableCollection<KeyValuePair<int, CatalogNode>>();
		}

		private ObservableCollection<KeyValuePair<int, CatalogNode>> _posts;

		public ObservableCollection<KeyValuePair<int, CatalogNode>> Posts
		{
			get => _posts;
			set
			{
				if (_posts == value) return;
				_posts = value;
				OnPropertyChanged();
			}
		}

		public void SetPosts(List<CatalogNode> posts)
		{
			Posts = new ObservableCollection<KeyValuePair<int, CatalogNode>>(posts.Select((post, index) =>
				new KeyValuePair<int, CatalogNode>(index, post)));
		}

		public event PropertyChangedEventHandler PropertyChanged;

		[NotifyPropertyChangedInvocator]
		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}