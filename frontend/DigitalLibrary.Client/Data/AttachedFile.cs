namespace DigitalLibrary.Client.Data
{
	public class AttachedFile
	{
		public string Id { get; set; }

		public string FileName { get; set; }

		public long Lenght { get; set; }
		
		public string LocalPath { get; set; }

		public string UserId { get; set; }
	}
}