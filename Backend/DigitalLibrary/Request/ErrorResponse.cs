using System.Collections.Generic;

namespace DigitalLibrary.Request
{
	public class ErrorResponse
	{
		public ErrorCodes ErrorCode { get; set; }

		public string ErrorMessage { get; set; }

		public Dictionary<string, string> RequestParameters { get; set; }
	}
}