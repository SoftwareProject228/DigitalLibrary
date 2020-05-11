using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.IO.Pipelines;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using BlazorInputFile;
using DigitalLibrary.Client.Annotations;
using DigitalLibrary.Client.Data;
using Newtonsoft.Json;

namespace DigitalLibrary.Client.ViewModels
{
	public class UploadPostViewModel : ILayoutItemViewModel, INotifyPropertyChanged
	{
		private readonly string _token;
		private bool _enteringTitle;
		private bool _enteringDescription;
		private bool _enteringFile;
		private bool _enteringTag;
		private Action _onFinish;

		public UploadPostViewModel(string token, Action onFinish)
		{
			_token = token;
			_onFinish = onFinish;
			EnteringTitle = true;
			Files = new ObservableCollection<IFileListEntry>();
		}

		public ItemType Type => ItemType.UploadPost;

		public ObservableCollection<IFileListEntry> Files { get; set; }

		public bool EnteringTag
		{
			get => _enteringTag;
			set
			{
				if (_enteringTag == value) return;
				_enteringTag = value;
				OnPropertyChanged();
			}
		}

		public bool EnteringTitle
		{
			get => _enteringTitle;
			set
			{
				if (_enteringTitle == value) return;
				_enteringTitle = value;
				OnPropertyChanged();
			}
		}

		public bool EnteringDescription
		{
			get => _enteringDescription;
			set
			{
				if (_enteringDescription == value) return;
				_enteringDescription = value;
				OnPropertyChanged();
			}
		}

		public bool EnteringFile
		{
			get => _enteringFile;
			set
			{
				if (_enteringFile == value) return;
				_enteringFile = value;
				OnPropertyChanged();
			}
		}

		public string Title { get; set; }

		public string Tag { get; set; }

		public string Description { get; set; }

		public async void Upload()
		{
			var files = new List<string>();
			foreach (var file in Files)
			{
				var stream = new MemoryStream();
				await file.Data.CopyToAsync(stream);
				var client = new HttpClient();
				var response = await client.PostAsync(
					$"https://localhost:44355/api/wall/uploadFile?token={_token}&fileName={file.Name}",
					new ByteArrayContent(stream.ToArray()));
				if (response.IsSuccessStatusCode)
				{
					var fileResponse =
						JsonConvert.DeserializeObject<FileResponse>(await response.Content.ReadAsStringAsync());
					files.Add(fileResponse.FileId);

				}
			}

			var post = new WallPost
			{
				Title = Title,
				Tags = new List<string>(new[] {Tag}),
				ContentText = Description,
				AttachedFilesId = files
			};
			var postClient = new HttpClient();
			var postResponse = await postClient.PostAsync($"https://localhost:44355/api/wall/uploadPost?token={_token}",
				new StringContent(JsonConvert.SerializeObject(post), Encoding.UTF8, "application/json"));
			if (postResponse.IsSuccessStatusCode)
			{
				_onFinish();
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