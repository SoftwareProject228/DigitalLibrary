using System.Collections.Generic;

namespace DigitalLibrary.Client.Components.Handlers
{
	public delegate void CommandEventHandler(string command, IDictionary<string, string> parameters);
}