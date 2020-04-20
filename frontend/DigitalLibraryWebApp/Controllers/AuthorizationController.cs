using System;
using System.Net.Http;
using System.Threading.Tasks;
using DigitalLibraryWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLibraryWebApp.Controllers
{
	public static class Globals
	{
		public const String Backend = "http://backend";
	}

	public class AuthorizationController : Controller
	{
		[HttpGet]
		public IActionResult Login(string regirectTo = null)
		{
			var viewModel = new LoginViewModel
			{
				RedirectTo = regirectTo
			};
			return View(viewModel);
		}

		[HttpPost]
		public async Task<IActionResult> Login(LoginViewModel vm)
		{
			if (string.IsNullOrWhiteSpace(vm.UserName) || string.IsNullOrWhiteSpace(vm.Password))
				return View(vm);

			var client = new HttpClient();
			client.DefaultRequestHeaders.Add("username", vm.UserName);
			client.DefaultRequestHeaders.Add("password", vm.Password);
			client.DefaultRequestHeaders.Add("application", "digitallibrary://web_app");
			var response = await client.GetAsync(Globals.Backend + "/api/authorization/login");
			if (response.IsSuccessStatusCode)
			{
				Response.Cookies.Append("token", await response.Content.ReadAsStringAsync());
				return RedirectToAction("Home", "Dashboard");
			}
			return View(vm);
		}

		[Route("signup/{linkToken?}")]
		[HttpGet]
		public async Task<IActionResult> SignUp(string linkToken)
		{
			if (String.IsNullOrWhiteSpace(linkToken))
				return RedirectToAction("Login");
			linkToken = linkToken.Replace("_", ".");
			var client = new HttpClient();
			client.DefaultRequestHeaders.Add("linktoken", linkToken);
			var response = await client.GetAsync(Globals.Backend + "/api/authorization/checklink");
			var valid = await response.Content.ReadAsStringAsync();
			if (valid == "false") return RedirectToAction("Login");

			var vm = new SignUpViewModel {LinkToken = linkToken};
			return View(vm);
		}

		[HttpPost]
		public async Task<IActionResult> SignUp(SignUpViewModel vm)
		{
			if (string.IsNullOrWhiteSpace(vm.UserName) || string.IsNullOrWhiteSpace(vm.Email) ||
			    string.IsNullOrWhiteSpace(vm.Password))
				return Content("ERROR");

			var client = new HttpClient();
			client.DefaultRequestHeaders.Add("linktoken", vm.LinkToken);
			client.DefaultRequestHeaders.Add("username", vm.UserName);
			client.DefaultRequestHeaders.Add("password", vm.Password);
			client.DefaultRequestHeaders.Add("email", vm.Email);
			client.DefaultRequestHeaders.Add("application", "digitallibrary://web_application");
			var response = await client.GetAsync(Globals.Backend + "/api/authorization/signup");
			if (response.IsSuccessStatusCode)
				return Content(await response.Content.ReadAsStringAsync());
			return Content("ERROR2");
		}
	}
}
