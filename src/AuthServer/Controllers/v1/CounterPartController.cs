using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers.v1;

public class CounterPartController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}