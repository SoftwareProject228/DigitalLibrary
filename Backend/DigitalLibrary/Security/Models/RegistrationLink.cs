﻿using System;
using DigitalLibrary.Authentication;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace DigitalLibrary.Security.Models
{
	public class RegistrationLink
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public string ForUserName { get; set; }

		public string ForEmail { get; set; }

		public UserRole ForUserStatus { get; set; }

		public bool Available { get; set; }

		public DateTime ExpiresAt { get; set; }

		[BsonRepresentation(BsonType.ObjectId)]
		public string CreatorUserId { get; set; }
	}
}