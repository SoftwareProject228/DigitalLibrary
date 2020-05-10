namespace DigitalLibrary.Request
{
	public enum ErrorCodes
	{
		MissingSomeArguments = 1,
		InvalidToken,
		TokenExpired,
		UserNotFound,
		IncorrectPassword,
		AccessDenied,
		FileNotFound,
		PostNotFound,
		RegistrationLinkNotAvailable,
		UserAlreadyExists
	}
}