﻿using System;
using System.IO;
using System.Threading.Tasks;
using DigitalLibrary.Context;
using DigitalLibrary.Security;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using SharedLibrary.Models;

namespace DigitalLibrary.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class IOController : ControllerBase
    {
        [HttpPost]
	    public async Task<IActionResult> UploadFile()
	    {
		    var token = Request.Headers["token"][0];
		    var validation = await UserWebTokenFactory.CheckTokenValidation(token);
		    if (validation != UserWebToken.TokenValidation.Valid)
		    {
			    return BadRequest(validation);
		    }

		    var fileName = Request.Headers["filename"][0];

		    var path = Path.Combine(Environment.CurrentDirectory, "files");
		    Directory.CreateDirectory(path);
		    path = Path.Combine(path, fileName);
		    var stream = System.IO.File.OpenWrite(path);
			await Request.Body.CopyToAsync(stream);
			stream.Flush();

			var attachedFile = new AttachedFile
			{
				FileName = fileName,
				Weight = stream.Length,
				LocalPath = path,
				UserToken = token
			};

			var filesCollection = MongoConnection.GetCollection<AttachedFile>("files");
			await filesCollection.InsertOneAsync(attachedFile);
			var filter = new BsonDocument("LocalPath", path);
			var found = await filesCollection.FindAsync(filter);
			var writtenFile = found.ToList()[0];
			attachedFile.Id = writtenFile.Id;
			return Ok(attachedFile.Id);
	    }

		[HttpPost]
	    public async Task<IActionResult> UploadPost([FromBody] WallPost post)
	    {
		    var token = Request.Headers["token"][0];
		    var validation = await UserWebTokenFactory.CheckTokenValidation(token);
		    if (validation != UserWebToken.TokenValidation.Valid)
			    return BadRequest(validation);

		    var postCollection = MongoConnection.GetCollection<WallPost>("posts");
		    post.UserToken = token;
		    postCollection.InsertOneAsync(post);
		    return Ok();
	    }
    }
}