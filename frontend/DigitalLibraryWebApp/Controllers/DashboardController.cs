using Microsoft.AspNetCore.Mvc;

namespace DigitalLibraryWebApp.Controllers
{
    public class DashboardController : Controller
    {
	    public IActionResult Home()
	    {
		    return View();
	    }
    }
}