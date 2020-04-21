using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DigitalLibrary.Context;
using DigitalLibrary.Model;
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
			    return BadRequest(new UserAuthenticationResponse
			    {
					AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.NotEnoughtHeaders
			    });

		    var username = Request.Headers["username"][0];
		    var password = Request.Headers["password"][0];
		    var application = Request.Headers["application"][0];

		    var passBytes = Encoding.UTF8.GetBytes(password);
		    var passHashBytes = MD5.Create().ComputeHash(passBytes);
		    var passHash = Convert.ToBase64String(passHashBytes);

		    var user = await UserContext.FindUserByNameAsync(username);

		    if (user == null)
			    return BadRequest(new UserAuthenticationResponse
			    {
					UserName = username,
					AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.UserNotFound
			    });
		    if (user.PasswordHash != passHash)
			    return BadRequest(new UserAuthenticationResponse
			    {
				    UserName = username,
				    AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.IncorrectPassword
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
				AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.Ok,
				UserStatus = user.Status
		    });
	    }

	    [HttpGet]
	    public IActionResult Logout()
	    {
		    if (!Request.Headers.ContainsKey("token"))
			    return BadRequest(new UserAuthenticationResponse
			    {
					AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.NotEnoughtHeaders
			    });

		    var token = Request.Headers["token"][0];

		    UserWebTokenFactory.DeleteTokenAsync(token);
		    return Ok();
	    }

		[HttpGet]
	    public async Task<IActionResult> CreateLink()
	    {
		    if (!Request.Headers.ContainsKey("token"))
			    return BadRequest(new UserAuthenticationResponse
			    {
					AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.NotEnoughtHeaders
			    });

		    var token = Request.Headers["token"][0];
		    var validation = await UserWebTokenFactory.CheckTokenValidationAsync(token);
		    if (validation == null || validation.Validation == UserWebToken.TokenValidation.Invalid)
			    return BadRequest(new UserAuthenticationResponse
			    {
					AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.InvalidToken
			    });
		    if (validation.Validation == UserWebToken.TokenValidation.Expired)
			    return BadRequest(new UserAuthenticationResponse
			    {
				    UserName = validation.User.UserName,
				    Email = validation.User.Email,
				    UserStatus = validation.User.Status,
				    Token = token,
				    AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.TokenExpired
			    });

		    if (validation.User.Status != Security.Models.User.UserStatus.Moderator)
			    return Unauthorized(new UserAuthenticationResponse
			    {
					UserName = validation.User.UserName,
					Email = validation.User.Email,
					Token = token,
					UserStatus = validation.User.Status,
					AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.AccessDenied
			    });

		    var status = Security.Models.User.UserStatus.Student;
			
		    if (Request.Headers.ContainsKey("status"))
			    status = Request.Headers["status"][0];
		    var expiresAt = DateTime.MaxValue;
			if (Request.Headers.ContainsKey("expiresAt"))
				expiresAt = DateTime.Now.AddDays(int.Parse(Request.Headers["expiresAt"][0]));
			var userName = "";
			if (Request.Headers.ContainsKey("username"))
				userName = Request.Headers["username"][0];
			var email = "";
			if (Request.Headers.ContainsKey("email"))
				email = Request.Headers["email"][0];

		    var userToken = UserWebToken.FromToken(token);

		    var linkId = await RegistrationLinkContext.AddLinkAsync(new RegistrationLink
		    {
			    Available = true,
			    ExpiresAt = expiresAt,
			    ForUserName = userName,
			    ForEmail = email,
			    ForUserStatus = status,
			    CreaterUserId = userToken.UserId
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
		public async Task<IActionResult> SignUp()
		{
			if (!Request.Headers.ContainsKey("linktoken"))
				return BadRequest(new RegistrationResponse
				{
					RegistrationStatus = RegistrationResponse.Status.NotEnoughtHeaders
				});

			var linkToken = Request.Headers["linktoken"][0];
			var link = await RegistrationLinkContext.FindLinkByIdAsync(linkToken);
			if (!link.Available || link.ExpiresAt < DateTime.Now)
				return BadRequest(new RegistrationResponse
				{
					RegistrationStatus = RegistrationResponse.Status.TokenNotAvailable
				});

			if (!Request.Headers.ContainsKey("username") || !Request.Headers.ContainsKey("password") ||
			    !Request.Headers.ContainsKey("email"))
				return BadRequest(new RegistrationResponse
				{
					RegistrationStatus = RegistrationResponse.Status.NotEnoughtHeaders
				});

			var userName = Request.Headers["username"][0];
			var foundUser = await UserContext.FindUserByNameAsync(userName);
			if (foundUser != null)
				return BadRequest(new RegistrationResponse
				{
					UserName = userName,
					RegistrationStatus = RegistrationResponse.Status.UserNameNotAvailable
				});

			var password = Request.Headers["password"][0];
			var email = Request.Headers["email"][0];


			var passBytes = Encoding.UTF8.GetBytes(password);
			var passHashBytes = MD5.Create().ComputeHash(passBytes);
			var passHash = Convert.ToBase64String(passHashBytes);

			await UserContext.AddUserAsync(new User
			{
				UserName = String.IsNullOrWhiteSpace(link.ForUserName) ? userName : link.ForUserName,
				Email = String.IsNullOrWhiteSpace(link.ForEmail) ? email : link.ForEmail,
				PasswordHash = passHash,
				Status = link.ForUserStatus,
				AssociatedTags = null,
				RegisteredByLink = link.Id
			});

			RegistrationLinkContext.InvalidateLinkAsync(link.Id);
			return Ok(new RegistrationResponse
			{
				UserName = String.IsNullOrWhiteSpace(link.ForUserName) ? userName : link.ForUserName,
				Email = String.IsNullOrWhiteSpace(link.ForEmail) ? email : link.ForEmail,
				UserStatus = link.ForUserStatus,
				RegistrationStatus = RegistrationResponse.Status.Ok
			});
		}

		[HttpGet]
		public async Task<IActionResult> CheckLink()
		{
			if (!Request.Headers.ContainsKey("linktoken"))
				return BadRequest();

			var linkToken = Request.Headers["linktoken"][0];

			var link = await RegistrationLinkContext.FindLinkByIdAsync(linkToken);
			return Ok(link);
		}
    }
}