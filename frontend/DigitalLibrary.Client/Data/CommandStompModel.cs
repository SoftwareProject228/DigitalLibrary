namespace DigitalLibrary.Client.Data
{
	public class CommandStompModel : ILayoutItem
	{
		public string UserName { get; set; }

		public string CommandText { get; set; }
		public ILayoutItem.ItemType Type { get; } = ILayoutItem.ItemType.CommandStomp;
	}
}
