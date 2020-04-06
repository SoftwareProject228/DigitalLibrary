using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DigitalLibrary.Context;
using DigitalLibrary.Security;
using DigitalLibrary.Security.Models;
using Microsoft.AspNetCore.Mvc;
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
    }
}