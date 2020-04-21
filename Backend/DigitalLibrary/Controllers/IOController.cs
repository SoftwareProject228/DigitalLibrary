using System;
using System.IO;
using System.Threading.Tasks;
using DigitalLibrary.Context;
using DigitalLibrary.Model;
using DigitalLibrary.Security;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLibrary.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IOController : ControllerBase
    {
        [HttpPost]
	    public async Task<IActionResult> UploadFile()
	    {
		    if (!Request.Headers.ContainsKey("token"))
			    return Unauthorized(new UserAuthenticationResponse
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


		    var fileName = Guid.NewGuid().ToString();
			if (Request.Headers.ContainsKey("filename"))
				fileName = Request.Headers["filename"][0];

		    var path = Path.Combine(Environment.CurrentDirectory, "files");
		    Directory.CreateDirectory(path);
		    path = Path.Combine(path, fileName);
		    var stream = System.IO.File.OpenWrite(path);
			await Request.Body.CopyToAsync(stream);
			stream.Flush();

			var attachedFile = new AttachedFile
			{
				FileName = fileName,
				Lenght = stream.Length,
				LocalPath = path,
				UserId = validation.User.Id
			};

			var fileId = await FileContext.AddFileAsync(attachedFile);
			return Ok(new NewFileResponse
			{
				FileId = fileId,
				FileName = fileName,
				FileLenght = attachedFile.Lenght
			});
	    }

		[HttpPost]
	    public async Task<IActionResult> UploadPost([FromBody] WallPost post)
	    {
			if (!Request.Headers.ContainsKey("token"))
				return Unauthorized(new UserAuthenticationResponse
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


			var postCollection = MongoConnection.GetCollection<WallPost>("posts");
		    post.UserToken = token;
		    postCollection.InsertOneAsync(post);
		    return Ok();
	    }
    }
}