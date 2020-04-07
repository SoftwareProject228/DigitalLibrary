using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DigitalLibrary.Context;
using DigitalLibrary.Security;
using DigitalLibrary.Security.Models;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;

namespace DigitalLibrary.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        [HttpGet]
	    public async Task<IActionResult> Login()
	    {
		    if (!Request.Headers.ContainsKey("username") || !Request.Headers.ContainsKey("password") ||
		        !Request.Headers.ContainsKey("application"))
		    {
			    return BadRequest("Missing some arguments for authorization");
		    }

		    var username = Request.Headers["username"][0];
		    var password = Request.Headers["password"][0];
		    var application = Request.Headers["application"][0];

		    var passBytes = Encoding.UTF8.GetBytes(password);
		    var passHashBytes = MD5.Create().ComputeHash(passBytes);
		    var passHash = Convert.ToBase64String(passHashBytes);
		    var userCollection = MongoConnection.GetCollection<User>("users");

		    var builder = Builders<User>.Filter;
		    var filter = builder.Eq("UserName", username) & builder.Eq("PasswordHash", passHash);
		    var user = await (await userCollection.FindAsync(filter)).FirstOrDefaultAsync();

		    if (user == null)
			    return BadRequest("User not found");

		    var userToken = new UserWebToken
		    {
			    Algorithm = "SHA256",
			    Application = application,
			    UserId = user.Id,
			    Host = "localhost",
			    ExpiresAt = DateTime.Now.AddHours(1)
		    };

			UserWebTokenFactory.AddTokenAsync(userToken);
		    return Ok(userToken.Token);
	    }

	    [HttpGet]
	    public async Task<IActionResult> Logout()
	    {
		    if (!Request.Headers.ContainsKey("token"))
			    return BadRequest("Missing argument: token");

		    var token = Request.Headers["token"][0];

		    await UserWebTokenFactory.DeleteTokenAsync(token);
		    return Ok();
	    }

		[HttpGet]
	    public async Task<IActionResult> CheckToken()
	    {
		    if (!Request.Headers.ContainsKey("token"))
			    return BadRequest("Missing argument: token");

		    var token = Request.Headers["token"][0];
		    var validation = await UserWebTokenFactory.CheckTokenValidationAsync(token);
		    return Ok(validation);
	    }

		[HttpGet]
	    public async Task<IActionResult> CreateLink()
	    {
		    if (!Request.Headers.ContainsKey("token"))
			    return BadRequest("Missing argument: token");

		    var token = Request.Headers["token"][0];
		    var validation = await UserWebTokenFactory.CheckTokenValidationAsync(token);
		    if (validation == null || validation.Validation != UserWebToken.TokenValidation.Valid)
			    return BadRequest(validation);

		    if (validation.UserStatus != Security.Models.User.UserStatus.Moderator)
			    return Unauthorized(validation);

		    var status = 0;
		    if (Request.Headers.ContainsKey("status"))
			    status = int.Parse(Request.Headers["status"][0]);
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

		    var registrationLinkWebToken = new RegistrationLinkWebToken
		    {
			    Status = (User.UserStatus) status,
			    ExpiresAt = expiresAt,
			    OwnerUserId = userToken.UserId,
			    UserName = userName,
			    Email = email,
			    Host = "localhost",
			    Algorithm = "SHA256",
			    Application = userToken.Application
		    };

		    RegistrationLinkWebTokenFactory.AddTokenAsync(registrationLinkWebToken, token);
		    return Ok(registrationLinkWebToken.Token);
	    }

		[HttpGet]
		public async Task<IActionResult> SignUp()
		{
			if (!Request.Headers.ContainsKey("linktoken"))
				return BadRequest("Missing argument: linktoken");

			var linkToken = Request.Headers["linktoken"];
			if (!await RegistrationLinkWebTokenFactory.CheckAvailabilityAsync(linkToken))
				return BadRequest("Token is not available");

			if (!Request.Headers.ContainsKey("username") || !Request.Headers.ContainsKey("password") ||
			    !Request.Headers.ContainsKey("email") || !Request.Headers.ContainsKey("application"))
				return BadRequest("Missing some arguments");

			var application = Request.Headers["application"][0];

			var userName = Request.Headers["username"][0];

			var userCollection = MongoConnection.GetCollection<User>("users");
			var findResults = await userCollection.FindAsync(new BsonDocument("UserName", userName));
			if (findResults.ToList().Count != 0)
				return BadRequest("Username already taken");

			var password = Request.Headers["password"][0];
			var email = Request.Headers["email"][0];


			var passBytes = Encoding.UTF8.GetBytes(password);
			var passHashBytes = MD5.Create().ComputeHash(passBytes);
			var passHash = Convert.ToBase64String(passHashBytes);

			var registrationLink = RegistrationLinkWebToken.FromToken(linkToken);
			var user = new User
			{
				UserName = userName,
				Email = email,
				PasswordHash = passHash,
				Status = registrationLink.Status,
				AssociatedTags = null
			};
			await userCollection.InsertOneAsync(user);

			var justAddedUser = (await userCollection.FindAsync(new BsonDocument("UserName", userName))).ToList()
				.First();

			var userToken = new UserWebToken
			{
				Application = application,
				UserId = justAddedUser.Id,
				Host = "localhost",
				Algorithm = "SHA256",
				ExpiresAt = DateTime.Now.AddHours(1)
			};

			UserWebTokenFactory.AddTokenAsync(userToken);
			RegistrationLinkWebTokenFactory.InvalidateLinkAsync(linkToken);
			return Ok(userToken.Token);
		}

		[HttpGet]
		public async Task<IActionResult> CheckLink()
		{
			if (!Request.Headers.ContainsKey("linktoken"))
				return BadRequest("Missing argument: linktoken");

			var linkToken = Request.Headers["linktoken"][0];

			var check = await RegistrationLinkWebTokenFactory.CheckAvailabilityAsync(linkToken);
			return Ok(check);
		}
    }
}