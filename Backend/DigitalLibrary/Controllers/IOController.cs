using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DigitalLibrary.Authentication;
using DigitalLibrary.Context;
using DigitalLibrary.Model;
using DigitalLibrary.Request;
using DigitalLibrary.Security;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLibrary.Controllers
{
    [Route("api/wall/[action]")]
    [ApiController]
    public class IOController : ControllerBase
    {
        [HttpPost]
	    public async Task<IActionResult> UploadFile(string token, string fileName)
	    {
		    if (String.IsNullOrWhiteSpace(token))
			    return BadRequest(new ErrorResponse
			    {
					ErrorCode = ErrorCodes.MissingSomeArguments,
					ErrorMessage = "Missing argument: token",
					RequestParameters = new Dictionary<string, string>(new []{new KeyValuePair<string, string>("method", "uploadFile") })
			    });

		    var validation = await UserWebTokenFactory.CheckTokenValidationAsync(token);
		    if (validation == null || validation.Validation == UserWebToken.TokenValidation.Invalid)
			    return BadRequest(new ErrorResponse
			    {
				    ErrorCode = ErrorCodes.InvalidToken,
				    ErrorMessage = "Invalid token",
				    RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "uploadFile") })
			    });
		    if (validation.Validation == UserWebToken.TokenValidation.Expired)
			    return BadRequest(new ErrorResponse
			    {
				    ErrorCode = ErrorCodes.TokenExpired,
				    ErrorMessage = "Token expired. Relogin required",
				    RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "uploadFile") })
			    });

			if (String.IsNullOrWhiteSpace(fileName))
				fileName = Guid.NewGuid().ToString();

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
			return Ok(new FileResponse
			{
				FileId = fileId,
				FileName = fileName,
				FileLenght = attachedFile.Lenght
			});
	    }

		[HttpPost]
	    public async Task<IActionResult> UploadPost(string token, [FromBody] WallPost post)
	    {
			if (String.IsNullOrWhiteSpace(token))
				return BadRequest(new ErrorResponse
				{
					ErrorCode = ErrorCodes.MissingSomeArguments,
					ErrorMessage = "Missing argument: token",
					RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "uploadPost") })
				});

			var validation = await UserWebTokenFactory.CheckTokenValidationAsync(token);
			if (validation == null || validation.Validation == UserWebToken.TokenValidation.Invalid)
				return BadRequest(new ErrorResponse
				{
					ErrorCode = ErrorCodes.InvalidToken,
					ErrorMessage = "Invalid token",
					RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "uploadPost") })
				});
			if (validation.Validation == UserWebToken.TokenValidation.Expired)
				return BadRequest(new ErrorResponse
				{
					ErrorCode = ErrorCodes.MissingSomeArguments,
					ErrorMessage = "Token expired. Relogin required",
					RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "uploadPost") })
				});


			CatalogContext.AddNewPostAsync(post, validation.User);
		    return Ok();
	    }

		[HttpGet]
	    public async Task<IActionResult> AcceptPost(string token, string postId)
	    {
		    if (String.IsNullOrWhiteSpace(token))
			    return BadRequest(new ErrorResponse
			    {
				    ErrorCode = ErrorCodes.MissingSomeArguments,
				    ErrorMessage = "Missing argument: token",
				    RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "acceptPost") })
			    });

		    var validation = await UserWebTokenFactory.CheckTokenValidationAsync(token);
		    if (validation == null || validation.Validation == UserWebToken.TokenValidation.Invalid)
			    return BadRequest(new ErrorResponse
			    {
				    ErrorCode = ErrorCodes.InvalidToken,
				    ErrorMessage = "Invalid token",
				    RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "acceptPost") })
			    });
		    if (validation.Validation == UserWebToken.TokenValidation.Expired)
			    return BadRequest(new ErrorResponse
			    {
				    ErrorCode = ErrorCodes.TokenExpired,
				    ErrorMessage = "Token expired. Relogin required",
				    RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "acceptPost") })
			    });

		    if (validation.User.Status == UserRole.Student)
			    return BadRequest(new ErrorResponse
			    {
				    ErrorCode = ErrorCodes.AccessDenied,
				    ErrorMessage = "Access denied",
				    RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "acceptPost") })
			    });

		    if (String.IsNullOrWhiteSpace(postId))
			    return BadRequest(new ErrorResponse
			    {
				    ErrorCode = ErrorCodes.AccessDenied,
				    ErrorMessage = "Missing argument: postId",
				    RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "acceptPost") })
			    });

		    if (validation.User.Status == UserRole.Professor)
		    {
			    var post = (await CatalogContext.SearchAsync(id: postId, catalog: "raw")).FirstOrDefault();
			    if (post.Tags.Union(validation.User.AssociatedTags).Count() ==
			        post.Tags.Count + validation.User.AssociatedTags.Count)
				    return BadRequest(new ErrorResponse
				    {
					    ErrorCode = ErrorCodes.AccessDenied,
					    ErrorMessage = "Access denied",
					    RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "acceptPost") })
				    });
		    }

		    CatalogContext.AcceptPostAsync(postId);
		    return Ok();
	    }

	    [HttpGet]
	    public async Task<IActionResult> DeclinePost(string token, string postId)
	    {
			if (String.IsNullOrWhiteSpace(token))
				return BadRequest(new ErrorResponse
				{
					ErrorCode = ErrorCodes.MissingSomeArguments,
					ErrorMessage = "Missing argument: token",
					RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "declinePost") })
				});

			var validation = await UserWebTokenFactory.CheckTokenValidationAsync(token);
			if (validation == null || validation.Validation == UserWebToken.TokenValidation.Invalid)
				return BadRequest(new ErrorResponse
				{
					ErrorCode = ErrorCodes.InvalidToken,
					ErrorMessage = "Invalid token",
					RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "declinePost") })
				});
			if (validation.Validation == UserWebToken.TokenValidation.Expired)
				return BadRequest(new ErrorResponse
				{
					ErrorCode = ErrorCodes.TokenExpired,
					ErrorMessage = "Token expired. Relogin required",
					RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "declinePost") })
				});

			if (validation.User.Status == UserRole.Student)
				return BadRequest(new ErrorResponse
				{
					ErrorCode = ErrorCodes.AccessDenied,
					ErrorMessage = "Access denied",
					RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "declinePost") })
				});

			if (String.IsNullOrWhiteSpace(postId))
				return BadRequest(new ErrorResponse
				{
					ErrorCode = ErrorCodes.MissingSomeArguments,
					ErrorMessage = "Missing argument: postId",
					RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "declinePost") })
				});

			if (validation.User.Status == UserRole.Professor)
			{
				var post = (await CatalogContext.SearchAsync(id: postId, catalog: "raw")).FirstOrDefault();
				if (post.Tags.Union(validation.User.AssociatedTags).Count() ==
					post.Tags.Count + validation.User.AssociatedTags.Count)
					return BadRequest(new ErrorResponse
					{
						ErrorCode = ErrorCodes.AccessDenied,
						ErrorMessage = "Access denied",
						RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "declinePost") })
					});
			}

			CatalogContext.DeclinePostAsync(postId);
			return Ok();
	    }

	    [HttpGet]
	    public async Task<IActionResult> DeletePost(string token, string postId)
	    {
			if (String.IsNullOrWhiteSpace(token))
				return BadRequest(new ErrorResponse
				{
					ErrorCode = ErrorCodes.MissingSomeArguments,
					ErrorMessage = "Missing argument: postId",
					RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "deletePost") })
				});

			var validation = await UserWebTokenFactory.CheckTokenValidationAsync(token);
			if (validation == null || validation.Validation == UserWebToken.TokenValidation.Invalid)
				return BadRequest(new ErrorResponse
				{
					ErrorCode = ErrorCodes.InvalidToken,
					ErrorMessage = "Invalid token",
					RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "deletePost") })
				});
			if (validation.Validation == UserWebToken.TokenValidation.Expired)
				return BadRequest(new ErrorResponse
				{
					ErrorCode = ErrorCodes.TokenExpired,
					ErrorMessage = "Token expired. Relogin required",
					RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "deletePost") })
				});

			if (String.IsNullOrWhiteSpace(postId))
				return BadRequest(new ErrorResponse
				{
					ErrorCode = ErrorCodes.MissingSomeArguments,
					ErrorMessage = "Missing argument: postId",
					RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "deletePost") })
				});

			var post = (await CatalogContext.SearchAsync(id: postId)).FirstOrDefault();

			if (validation.User.Status == UserRole.Student && post.Publisher.Id != validation.User.Id)
				return BadRequest(new ErrorResponse
				{
					ErrorCode = ErrorCodes.AccessDenied,
					ErrorMessage = "Access denied",
					RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "deletePost") })
				});


			if (validation.User.Status == UserRole.Professor)
			{
				if (post.Tags.Union(validation.User.AssociatedTags).Count() ==
					post.Tags.Count + validation.User.AssociatedTags.Count)
					return BadRequest(new ErrorResponse
					{
						ErrorCode = ErrorCodes.AccessDenied,
						ErrorMessage = "Access denied",
						RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "deletePost") })
					});
			}

			CatalogContext.DeletePostAsync(postId);
			return Ok();
	    }

	    [HttpGet]
	    public async Task<IActionResult> Search(string token, string title, string tag, string id,
		    string catalog = "primary")
	    {
		    if (String.IsNullOrWhiteSpace(token))
			    return BadRequest(new ErrorResponse
			    {
				    ErrorCode = ErrorCodes.MissingSomeArguments,
				    ErrorMessage = "Missing argument: token",
				    RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "search") })
			    });

		    var validation = await UserWebTokenFactory.CheckTokenValidationAsync(token);
		    if (validation == null || validation.Validation == UserWebToken.TokenValidation.Invalid)
			    return BadRequest(new ErrorResponse
			    {
				    ErrorCode = ErrorCodes.InvalidToken,
				    ErrorMessage = "Invalid token",
				    RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "search") })
			    });
		    if (validation.Validation == UserWebToken.TokenValidation.Expired)
			    return BadRequest(new ErrorResponse
			    {
				    ErrorCode = ErrorCodes.TokenExpired,
				    ErrorMessage = "Token expired. Relogin required",
				    RequestParameters = new Dictionary<string, string>(new[] { new KeyValuePair<string, string>("method", "search") })
			    });

			var posts = await CatalogContext.SearchAsync(title, tag, id, catalog);
		    return Ok(posts);
	    }

	}
}