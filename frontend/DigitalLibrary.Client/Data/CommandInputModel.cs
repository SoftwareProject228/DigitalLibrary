namespace DigitalLibrary.Client.Data
{
	public class CommandInputModel : ILayoutItem
	{
		public ILayoutItem.ItemType Type { get; } = ILayoutItem.ItemType.CommandItem;
	}
}
