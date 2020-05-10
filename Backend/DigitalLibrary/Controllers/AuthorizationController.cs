using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DigitalLibrary.Authentication;
using DigitalLibrary.Context;
using DigitalLibrary.Model;
using DigitalLibrary.Request;
using DigitalLibrary.Security;
using DigitalLibrary.Security.Models;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLibrary.Controllers
{
    [Route("api/auth/[action]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        [HttpGet]
	    public async Task<IActionResult> Login()
	    {
		    if (!Request.Headers.ContainsKey("username") || !Request.Headers.ContainsKey("password") ||
		        !Request.Headers.ContainsKey("application"))
			    return BadRequest(new ErrorResponse
			    {
					ErrorCode = ErrorCodes.MissingSomeArguments,
					ErrorMessage = "Missing arguments: username, password or appplication",
					RequestParameters = new Dictionary<string, string>(new []{new KeyValuePair<string, string>("method", "login") })
			    });

		    var username = Request.Headers["username"][0];
		    var password = Request.Headers["password"][0];
		    var application = Request.Headers["application"][0];

		    var passBytes = Encoding.UTF8.GetBytes(password);
		    var passHashBytes = MD5.Create().ComputeHash(passBytes);
		    var passHash = Convert.ToBase64String(passHashBytes);

		    var user = await UserContext.FindUserByNameAsync(username);

		    if (user == null)
			    return BadRequest(new ErrorResponse
			    {
					ErrorCode = ErrorCodes.UserNotFound,
					ErrorMessage = "User with given credintails not found",
					RequestParameters = new Dictionary<string, string>(new []{new KeyValuePair<string, string>("method", "login") })
			    });
		    if (user.PasswordHash != passHash)
			    return BadRequest(new ErrorResponse
			    {
					ErrorCode = ErrorCodes.IncorrectPassword,
					ErrorMessage = "Inccorect password",
					RequestParameters = new Dictionary<string, string>(new []{new KeyValuePair<string, string>("method", "login"), })
			    });

		    var userToken = new UserWebToken
		    {
			    Algorithm = "SHA256",
			    Application = application,
			    UserId = user.Id,
			    Host = "localhost",
			    ExpiresAt = DateTime.Now.AddHours(1)
		    };

			UserWebTokenFactory.AddTokenAsync(userToken);
		    return Ok(new UserAuthenticationResponse
		    {
				UserName = user.UserName,
				Email = user.Email,
				Token = userToken.Token,
				UserRole = user.Status
		    });
	    }

	    [HttpGet]
	    public IActionResult Logout(string token)
	    {
		    if (String.IsNullOrWhiteSpace(token))
			    return BadRequest(new ErrorResponse
			    {
					ErrorCode = ErrorCodes.MissingSomeArguments,
					ErrorMessage = "Missing argument: token",
					RequestParameters = new Dictionary<string, string>(new []{new KeyValuePair<string, string>("method", "logout") })
			    });

		    UserWebTokenFactory.DeleteTokenAsync(token);
		    return Ok();
	    }

		[HttpGet]
	    public async Task<IActionResult> CreateLink(string token, int? userRole, int? expiresIn, string userName, string email)
	    {
		    if (String.IsNullOrWhiteSpace(token))
			    return BadRequest(new ErrorResponse
			    {
					ErrorCode = ErrorCodes.MissingSomeArguments,
					ErrorMessage = "Miising argument: token",
					RequestParameters = new Dictionary<string, string>(new []{new KeyValuePair<string, string>("method", "createLink") })
			    });

		    var validation = await UserWebTokenFactory.CheckTokenValidationAsync(token);
		    if (validation == null || validation.Validation == UserWebToken.TokenValidation.Invalid)
			    return BadRequest(new ErrorResponse
			    {
					ErrorCode = ErrorCodes.InvalidToken,
					ErrorMessage = "Invalid authentication token",
					RequestParameters = new Dictionary<string, string>(new []
					{
						new KeyValuePair<string, string>("method", "createLink"),
					})
			    });
		    if (validation.Validation == UserWebToken.TokenValidation.Expired)
			    return BadRequest(new ErrorResponse
			    {
					ErrorCode = ErrorCodes.TokenExpired,
					ErrorMessage = "Token expired. Relogin required",
					RequestParameters = new Dictionary<string, string>(new []{new KeyValuePair<string, string>("method", "createLink") })
			    });

		    if (validation.User.Status != UserRole.Moderator)
			    return BadRequest(new ErrorResponse
			    {
					ErrorCode = ErrorCodes.AccessDenied,
					ErrorMessage = "Access denied. Minimum access level: moderator",
					RequestParameters = new Dictionary<string, string>(new []{new KeyValuePair<string, string>("method", "createLink") })
			    });

		    var status = UserRole.Student;

		    if (userRole != null)
			    status = (UserRole) userRole;

		    var expiresAt = DateTime.MaxValue;
			if (expiresIn != null)
				expiresAt = DateTime.Now.AddHours(expiresIn.Value);

			if (String.IsNullOrWhiteSpace(userName))
				userName = "";

			if (String.IsNullOrWhiteSpace(email))
				email = "";

		    var userToken = UserWebToken.FromToken(token);

		    var linkId = await RegistrationLinkContext.AddLinkAsync(new RegistrationLink
		    {
			    Available = true,
			    ExpiresAt = expiresAt,
			    ForUserName = userName,
			    ForEmail = email,
			    ForUserStatus = status,
			    CreatorUserId = userToken.UserId
		    });

		    return Ok(new RegistrationLinkResponse
		    {
				ForUserName = userName,
				ForEmail = email,
				ForUserStatus = status,
				LinkToken = linkId,
				ExpiresAt = expiresAt
		    });
	    }

		[HttpGet]
		public async Task<IActionResult> SignUp(string link)
		{
			if (String.IsNullOrWhiteSpace(link))
				return BadRequest(new ErrorResponse
				{
					ErrorCode = ErrorCodes.MissingSomeArguments,
					ErrorMessage = "Missing argument: link",
					RequestParameters = new Dictionary<string, string>(new []{new KeyValuePair<string, string>("method", "signUp") })
				});

			var linkToken = await RegistrationLinkContext.FindLinkByIdAsync(link);
			if (!linkToken.Available || linkToken.ExpiresAt < DateTime.Now)
				return BadRequest(new ErrorResponse
				{
					ErrorCode = ErrorCodes.RegistrationLinkNotAvailable,
					ErrorMessage = "The link is not available",
					RequestParameters = new Dictionary<string, string>(new []{new KeyValuePair<string, string>("method", "signUp") })
				});

			if (!Request.Headers.ContainsKey("username") || !Request.Headers.ContainsKey("password") ||
			    !Request.Headers.ContainsKey("email"))
				return BadRequest(new ErrorResponse
				{
					ErrorCode = ErrorCodes.MissingSomeArguments,
					ErrorMessage = "Missing some arguments: username, password or email",
					RequestParameters = new Dictionary<string, string>(new []{new KeyValuePair<string, string>("method", "signUp") })
				});

			var userName = Request.Headers["username"][0];
			var foundUser = await UserContext.FindUserByNameAsync(userName);
			if (foundUser != null)
				return BadRequest(new ErrorResponse
				{
					ErrorCode = ErrorCodes.UserAlreadyExists,
					ErrorMessage = "User with selected user name already exists",
					RequestParameters = new Dictionary<string, string>(new []{new KeyValuePair<string, string>("method", "signUp") })
				});

			var password = Request.Headers["password"][0];
			var email = Request.Headers["email"][0];


			var passBytes = Encoding.UTF8.GetBytes(password);
			var passHashBytes = MD5.Create().ComputeHash(passBytes);
			var passHash = Convert.ToBase64String(passHashBytes);

			await UserContext.AddUserAsync(new User
			{
				UserName = String.IsNullOrWhiteSpace(linkToken.ForUserName) ? userName : linkToken.ForUserName,
				Email = String.IsNullOrWhiteSpace(linkToken.ForEmail) ? email : linkToken.ForEmail,
				PasswordHash = passHash,
				Status = linkToken.ForUserStatus,
				AssociatedTags = null,
				RegisteredByLink = linkToken.Id
			});

			RegistrationLinkContext.InvalidateLinkAsync(linkToken.Id);
			return Ok(new RegistrationResponse
			{
				UserName = String.IsNullOrWhiteSpace(linkToken.ForUserName) ? userName : linkToken.ForUserName,
				Email = String.IsNullOrWhiteSpace(linkToken.ForEmail) ? email : linkToken.ForEmail,
				UserStatus = linkToken.ForUserStatus,
			});
		}

		[HttpGet]
		public async Task<IActionResult> CheckLink(string link)
		{
			if (String.IsNullOrWhiteSpace(link))
				return BadRequest(new ErrorResponse
				{
					ErrorCode = ErrorCodes.MissingSomeArguments,
					ErrorMessage = "Missing argument: link",
					RequestParameters = new Dictionary<string, string>(new []{new KeyValuePair<string, string>("method", "checkLink") })
				});

			var linkToken = await RegistrationLinkContext.FindLinkByIdAsync(link);
			return Ok(linkToken);
		}
    }
}