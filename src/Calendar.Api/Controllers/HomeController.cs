using Microsoft.AspNetCore.Mvc;

namespace Calendar.Api.Controllers;

public class HomeController : Controller
{
    public IActionResult Index() => new RedirectResult("~/swagger");
}