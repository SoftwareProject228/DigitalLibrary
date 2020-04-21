namespace DigitalLibrary.Model
{
	public class UserAuthenticationResponse
	{
		public static class UserAuthenticationStatus
		{
			public static string Ok { get; } = "ok";
			public static string TokenExpired { get; } = "token_expired";
			public static string UserNotFound { get; } = "user_not_found";
			public static string IncorrectPassword { get; } = "incorrect_password";
			public static string NotEnoughtHeaders { get; } = "not_enought_headers";
			public static string AccessDenied { get; } = "access_denied";
			public static string InvalidToken { get; } = "invalid_token";
		}

		public string UserName { get; set; }

		public string Email { get; set; }

		public string UserStatus { get; set; }

		public string AuthenticationStatus { get; set; }

		public string Token { get; set; }
	}
}