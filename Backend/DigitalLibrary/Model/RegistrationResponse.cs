namespace DigitalLibrary.Model
{
	public class RegistrationResponse
	{
		public static class Status
		{
			public static string Ok { get; } = "ok";
			public static string NotEnoughtHeaders { get; } = "not_enought_headers";
			public static string TokenNotAvailable { get; } = "token_is_not_available";
			public static string UserNameNotAvailable { get; } = "username_not_available";
		}
		public string UserName { get; set; }

		public string UserStatus { get; set; }

		public string Email { get; set; }

		public string RegistrationStatus { get; set; }
	}
}