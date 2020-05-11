using System.Collections.Generic;

namespace DigitalLibrary.Client.Data
{
	public class WallPost
	{
		public string Title { get; set; }

		public string ContentText { get; set; }

		public List<string> Tags { get; set; }

		public List<string> AttachedFilesId { get; set; }
	}
}