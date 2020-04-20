using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace DatabaseControl
{
	public class Program
	{
		private static async Task Main()
		{
			var client = new MongoClient(@"mongodb+srv://DigitalLibrary:23081975@mongospace-1jtjh.azure.mongodb.net/test?retryWrites=true&w=majority");
			var db = client.GetDatabase("digital_library_storage");

			await db.CreateCollectionAsync("posts");
		}
	}
}