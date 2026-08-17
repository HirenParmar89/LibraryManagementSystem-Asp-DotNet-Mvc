using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace LibraryManagementSystem.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return RedirectToAction("Index", "Dashboard");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        // Handles 500 errors caught by middleware
        return View();
    }

    [HttpGet("/Home/StatusCode")]
    [AllowAnonymous]
    public IActionResult StatusCode(int? code = null)
    {
        // Handles 404 and 403 redirects
        if (code == 404)
        {
            return View("NotFound");
        }
        if (code == 403)
        {
            return View("AccessDenied");
        }
        
        return View("Error");
    }
}