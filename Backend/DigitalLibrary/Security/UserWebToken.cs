﻿using System;
using System.Security.Cryptography;
using System.Text;
using DigitalLibrary.Properties;
using Newtonsoft.Json;

namespace DigitalLibrary.Security
{
	public class UserWebToken
	{
		public enum TokenValidation
		{
			Valid,
			Expired,
			Invalid
		}

		private static string Base64UrlEncode(byte[] input)
		{
			var output = Convert.ToBase64String(input);
			output = output.Split('=')[0]; // Remove any trailing '='s
			output = output.Replace('+', '-'); // 62nd char of encoding
			output = output.Replace('/', '_'); // 63rd char of encoding
			return output;
		}

		private static byte[] Base64UrlDecode(string input)
		{
			var output = input;
			output = output.Replace('-', '+'); // 62nd char of encoding
			output = output.Replace('_', '/'); // 63rd char of encoding
			switch (output.Length % 4) // Pad with trailing '='s
			{
				case 0: break; // No pad chars in this case
				case 2:
					output += "==";
					break; // Two pad chars
				case 3:
					output += "=";
					break; // One pad char
				default: throw new Exception("Illegal base64url string");
			}

			var converted = Convert.FromBase64String(output); // Standard base64 decoder
			return converted;
		}

		private string CreateToken()
		{
			var secret = Encoding.UTF8.GetBytes(Resource.UserWebTokenSecret);
			var header = new JWTHead
			{
				Algorithm = Algorithm,
				Host = Host,
				ExpiresAt = ExpiresAt,
				Application = Application
			};
			var payload = new UserPayload
			{
				UserId = UserId
			};

			var headerBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(header));
			var payloadBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(payload));

			var encodedHead = Base64UrlEncode(headerBytes);
			var encodedPayload = Base64UrlEncode(payloadBytes);

			var stringsToSign = encodedHead + '.' + encodedPayload;
			var bytesToSign = Encoding.UTF8.GetBytes(stringsToSign);

			var signatureBytes = new HMACSHA256(secret).ComputeHash(bytesToSign);
			var signature = Base64UrlEncode(signatureBytes);

			return stringsToSign + '.' + signature;
		}

		private static UserWebToken ResolveToken(string token)
		{
			var parts = token.Split('.');
			var header = parts[0];
			var payload = parts[1];
			var crypto = Base64UrlDecode(parts[2]);

			var headerJson = Encoding.UTF8.GetString(Base64UrlDecode(header));
			var payloadJson = Encoding.UTF8.GetString(Base64UrlDecode(payload));

			var headerData = JsonConvert.DeserializeObject<JWTHead>(headerJson);
			var payloadData = JsonConvert.DeserializeObject<UserPayload>(payloadJson);

			if (headerData.Algorithm != "SHA256")
				throw new Exception("Unknown hashing algorithm");
			var bytesToSign = Encoding.UTF8.GetBytes(header + '.' + payload);
			var secret = Encoding.UTF8.GetBytes(Resource.UserWebTokenSecret);

			var signature = new HMACSHA256(secret).ComputeHash(bytesToSign);

			var decodedCrypto = Convert.ToBase64String(crypto);
			var decodedSignature = Convert.ToBase64String(signature);

			if (decodedCrypto != decodedSignature)
			{
				throw new ApplicationException("Invalid signature");
			}

			var jwt = new UserWebToken
			{
				Algorithm = headerData.Algorithm,
				ExpiresAt = headerData.ExpiresAt,
				Host = headerData.Host,
				Application = headerData.Application,
				UserId = payloadData.UserId
			};

			return jwt;
		}

		private string _token;
		private bool _dataChanged = true;
		private string _algorithm;
		private string _host;
		private DateTime _expiresAt;
		private string _application;
		private string _userId;

		public static UserWebToken FromToken(string token)
		{
			return ResolveToken(token);
		}

		public string Algorithm
		{
			get => _algorithm;
			set
			{
				if (_algorithm == value) return;
				_algorithm = value;
				_dataChanged = true;
			}
		}

		public string Host
		{
			get => _host;
			set
			{
				if (_host == value) return;
				_host = value;
				_dataChanged = true;
			}
		}

		public DateTime ExpiresAt
		{
			get => _expiresAt;
			set
			{
				if (_expiresAt == value) return;
				_expiresAt = value;
				_dataChanged = true;
			}
		}

		public string Application
		{
			get => _application;
			set
			{
				if (_application == value) return;
				_application = value;
				_dataChanged = true;
			}
		}

		public string UserId
		{
			get => _userId;
			set
			{
				if (_userId == value) return;
				_userId = value;
				_dataChanged = true;
			}
		}

		public string Token
		{
			get
			{
				if (!_dataChanged) return _token;
				_token = CreateToken();
				return _token;
			}
		}
	}
}