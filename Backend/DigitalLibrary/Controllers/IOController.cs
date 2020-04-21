using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DigitalLibrary.Context;
using DigitalLibrary.Model;
using DigitalLibrary.Security;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLibrary.Controllers
{
    [Route("api/wall/[action]")]
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


			CatalogContext.AddNewPostAsync(post, validation.User);
		    return Ok();
	    }

		[HttpGet]
	    public async Task<IActionResult> AcceptPost()
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

		    if (validation.User.Status == Security.Models.User.UserStatus.Student)
			    return BadRequest(new UserAuthenticationResponse
			    {
				    UserName = validation.User.UserName,
				    Email = validation.User.Email,
				    UserStatus = validation.User.Status,
				    Token = token,
				    AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.AccessDenied
			    });

		    if (!Request.Headers.ContainsKey("post_id"))
			    return BadRequest(new UserAuthenticationResponse
			    {
				    UserName = validation.User.UserName,
				    Email = validation.User.Email,
				    UserStatus = validation.User.Status,
				    Token = token,
				    AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.NotEnoughtHeaders
			    });

		    var postId = Request.Headers["post_id"];
		    if (validation.User.Status == Security.Models.User.UserStatus.Professor)
		    {
			    var post = await CatalogContext.FindPostByIdRawAsync(postId);
			    if (post.Tags.Union(validation.User.AssociatedTags).Count() ==
			        post.Tags.Count + validation.User.AssociatedTags.Count)
				    return BadRequest(new UserAuthenticationResponse
				    {
					    UserName = validation.User.UserName,
					    Email = validation.User.Email,
					    UserStatus = validation.User.Status,
					    Token = token,
					    AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.AccessDenied
				    });
		    }

		    CatalogContext.AcceptPostAsync(postId);
		    return Ok();
	    }

	    [HttpGet]
	    public async Task<IActionResult> DeclinePost()
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

			if (validation.User.Status == Security.Models.User.UserStatus.Student)
				return BadRequest(new UserAuthenticationResponse
				{
					UserName = validation.User.UserName,
					Email = validation.User.Email,
					UserStatus = validation.User.Status,
					Token = token,
					AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.AccessDenied
				});

			if (!Request.Headers.ContainsKey("post_id"))
				return BadRequest(new UserAuthenticationResponse
				{
					UserName = validation.User.UserName,
					Email = validation.User.Email,
					UserStatus = validation.User.Status,
					Token = token,
					AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.NotEnoughtHeaders
				});

			var postId = Request.Headers["post_id"];
			if (validation.User.Status == Security.Models.User.UserStatus.Professor)
			{
				var post = await CatalogContext.FindPostByIdRawAsync(postId);
				if (post.Tags.Union(validation.User.AssociatedTags).Count() ==
					post.Tags.Count + validation.User.AssociatedTags.Count)
					return BadRequest(new UserAuthenticationResponse
					{
						UserName = validation.User.UserName,
						Email = validation.User.Email,
						UserStatus = validation.User.Status,
						Token = token,
						AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.AccessDenied
					});
			}

			CatalogContext.DeclinePostAsync(postId);
			return Ok();
	    }

	    [HttpGet]
	    public async Task<IActionResult> DeletePost()
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

			if (validation.User.Status == Security.Models.User.UserStatus.Student)
				return BadRequest(new UserAuthenticationResponse
				{
					UserName = validation.User.UserName,
					Email = validation.User.Email,
					UserStatus = validation.User.Status,
					Token = token,
					AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.AccessDenied
				});

			if (!Request.Headers.ContainsKey("post_id"))
				return BadRequest(new UserAuthenticationResponse
				{
					UserName = validation.User.UserName,
					Email = validation.User.Email,
					UserStatus = validation.User.Status,
					Token = token,
					AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.NotEnoughtHeaders
				});

			var postId = Request.Headers["post_id"];
			if (validation.User.Status == Security.Models.User.UserStatus.Professor)
			{
				var post = await CatalogContext.FindPostByIdRawAsync(postId);
				if (post.Tags.Union(validation.User.AssociatedTags).Count() ==
					post.Tags.Count + validation.User.AssociatedTags.Count)
					return BadRequest(new UserAuthenticationResponse
					{
						UserName = validation.User.UserName,
						Email = validation.User.Email,
						UserStatus = validation.User.Status,
						Token = token,
						AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.AccessDenied
					});
			}

			CatalogContext.DeletePostAsync(postId);
			return Ok();
	    }

	    [HttpGet]
	    public async Task<IActionResult> FindPostsByTag()
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

			if (!Request.Headers.ContainsKey("tag"))
				return BadRequest(new UserAuthenticationResponse
				{
					UserName = validation.User.UserName,
					Email = validation.User.Email,
					UserStatus = validation.User.Status,
					Token = token,
					AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.NotEnoughtHeaders
				});

			var tag = Request.Headers["tag"];
			var posts = await CatalogContext.FindPostsByTagPrimaryAsync(tag);
			return Ok(posts);
	    }

	    [HttpGet]
	    public async Task<IActionResult> FindPostsByTagRaw()
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

			if (validation.User.Status == Security.Models.User.UserStatus.Student)
				return BadRequest(new UserAuthenticationResponse
				{
					UserName = validation.User.UserName,
					Email = validation.User.Email,
					UserStatus = validation.User.Status,
					Token = token,
					AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.AccessDenied
				});

			if (!Request.Headers.ContainsKey("tag"))
				return BadRequest(new UserAuthenticationResponse
				{
					UserName = validation.User.UserName,
					Email = validation.User.Email,
					UserStatus = validation.User.Status,
					Token = token,
					AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.NotEnoughtHeaders
				});

			var tag = Request.Headers["tag"];
			if (validation.User.Status == Security.Models.User.UserStatus.Professor)
			{
				if (validation.User.AssociatedTags == null || !validation.User.AssociatedTags.Contains(tag))
					return BadRequest(new UserAuthenticationResponse
					{
						UserName = validation.User.UserName,
						Email = validation.User.Email,
						UserStatus = validation.User.Status,
						Token = token,
						AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.AccessDenied
					});
			}

			var posts = await CatalogContext.FindPostsByTagRawAsync(tag);
			return Ok(posts);
	    }

		[HttpGet]
	    public async Task<IActionResult> FindPostByIdRaw()
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

			if (validation.User.Status == Security.Models.User.UserStatus.Student)
				return BadRequest(new UserAuthenticationResponse
				{
					UserName = validation.User.UserName,
					Email = validation.User.Email,
					UserStatus = validation.User.Status,
					Token = token,
					AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.AccessDenied
				});

			if (!Request.Headers.ContainsKey("post_id"))
				return BadRequest(new UserAuthenticationResponse
				{
					UserName = validation.User.UserName,
					Email = validation.User.Email,
					UserStatus = validation.User.Status,
					Token = token,
					AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.NotEnoughtHeaders
				});

			var postId = Request.Headers["post_id"];
			if (validation.User.Status == Security.Models.User.UserStatus.Professor)
			{
				var post = await CatalogContext.FindPostByIdRawAsync(postId);
				if (post.Tags.Union(validation.User.AssociatedTags).Count() ==
					post.Tags.Count + validation.User.AssociatedTags.Count)
					return BadRequest(new UserAuthenticationResponse
					{
						UserName = validation.User.UserName,
						Email = validation.User.Email,
						UserStatus = validation.User.Status,
						Token = token,
						AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.AccessDenied
					});
			}

			var post2 = await CatalogContext.FindPostByIdRawAsync(postId);
			return Ok(post2);
	    }

	    [HttpGet]
	    public async Task<IActionResult> FindPostById()
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

			if (!Request.Headers.ContainsKey("post_id"))
				return BadRequest(new UserAuthenticationResponse
				{
					UserName = validation.User.UserName,
					Email = validation.User.Email,
					UserStatus = validation.User.Status,
					Token = token,
					AuthenticationStatus = UserAuthenticationResponse.UserAuthenticationStatus.NotEnoughtHeaders
				});

			var postId = Request.Headers["post_id"];
			var post = await CatalogContext.FindPostByIdPrimaryAsync(postId);
			return Ok(post);
	    }
	}
}