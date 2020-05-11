using System;
using System.Collections.Generic;

namespace DigitalLibrary.Client.Data
{
	public class CatalogNode
	{
		public string Id { get; set; }

		public DateTime CreationTime { get; set; }

		public string Title { get; set; }

		public string ContentText { get; set; }

		public List<string> Tags { get; set; }

		public User Publisher { get; set; }

		public List<AttachedFile> AttachedFiles { get; set; }
	}
}