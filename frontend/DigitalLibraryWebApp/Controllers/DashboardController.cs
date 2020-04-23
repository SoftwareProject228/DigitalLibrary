using DigitalLibraryWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace DigitalLibraryWebApp.Controllers
{
    public class DashboardController : Controller
    {
		[HttpGet]
	    public IActionResult Home()
	    {
		    return View(new CommandViewModel());
	    }

	    private IActionResult ChooseAction(string action)
	    {
		    switch (action)
		    {
				case "help":
					return RedirectToAction("Help");
				case "delete":
					return RedirectToAction("Delete");
				case "library":
					return RedirectToAction("Library");
				case "home":
					return RedirectToAction("Home");
				default:
					return RedirectToAction("Error");
		    }
	    }

	    [HttpPost]
	    public IActionResult Home(CommandViewModel vm)
	    {
		    return ChooseAction(vm.CommandR);
	    }

	    [HttpPost]
	    public IActionResult Help(CommandViewModel vm)
	    {
		    return ChooseAction(vm.CommandR);
	    }

	    [HttpPost]
	    public IActionResult Delete(CommandViewModel vm)
	    {
		    return ChooseAction(vm.CommandR);
	    }

	    [HttpPost]
	    public IActionResult Library(CommandViewModel vm)
	    {
		    return ChooseAction(vm.CommandR);
	    }

	    [HttpPost]
	    public IActionResult Error(CommandViewModel vm)
	    {
		    return ChooseAction(vm.CommandR);
	    }


		[HttpGet]
	    public IActionResult Help()
	    {
		    return View(new CommandViewModel());
	    }

		[HttpGet]
	    public IActionResult Delete()
	    {
		    return View(new CommandViewModel());
	    }

		[HttpGet]
	    public IActionResult Library()
	    {
		    return View(new CommandViewModel());
	    }

		[HttpGet]
	    public IActionResult Error()
	    {
		    return View(new CommandViewModel());
	    }
    }
}