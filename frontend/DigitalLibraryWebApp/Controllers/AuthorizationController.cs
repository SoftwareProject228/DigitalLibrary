using System.Net.Http;
using System.Threading.Tasks;
using DigitalLibraryWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLibraryWebApp.Controllers
{
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
			var response = await client.GetAsync("https://localhost:44355/api/authorization/login");
			if (response.IsSuccessStatusCode)
			{
				Response.Cookies.Append("token", await response.Content.ReadAsStringAsync());
				return RedirectToAction("Home", "Dashboard");
			}
			return View(vm);
		}
	}
}