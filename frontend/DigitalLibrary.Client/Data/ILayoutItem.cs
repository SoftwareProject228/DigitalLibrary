namespace DigitalLibrary.Client.Data
{
	public interface ILayoutItem
	{
		enum ItemType
		{
			CommandItem,
			CommandStomp
		}

		ItemType Type { get; }
	}
}