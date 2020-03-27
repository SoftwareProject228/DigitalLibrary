namespace DigitalLibrary.Security
{
	public class UserPayload
	{
		public enum UserStatus
		{
			Student,
			Moderator,
			Professor
		}

		public string UserName { get; set; }

		public UserStatus Status { get; set; }
	}
}