using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLibrary.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        [HttpGet]
	    public IActionResult Login()
	    {
		    var username = Request.Headers["username"][0];
		    var password = Request.Headers["password"][0];

		    var passBytes = Encoding.UTF8.GetBytes(password);
		    var passHashBytes = MD5.Create().ComputeHash(passBytes);
		    var passHash = Convert.ToBase64String(passHashBytes);
		    return null;

	    }
    }
}