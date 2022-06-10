using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using cis2055_nemesys.Models;
using Microsoft.AspNetCore.Identity;

namespace cis2055_nemesys.Controllers;

public class HomeController : Controller
{
    private SignInManager<User> _signInManager;

    public HomeController(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
        
    }

    /// <summary>
    /// Redirect to Dashboard if signed in otherwise redirect to login page.
    /// </summary>
    public IActionResult Index()
    {
        if (_signInManager.IsSignedIn(User)) {
            return RedirectToAction(actionName: "Index", controllerName: "Dashboard");
        }

        return Redirect("/Identity/Account/Login");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

