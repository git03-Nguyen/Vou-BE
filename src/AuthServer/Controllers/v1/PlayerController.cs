using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controllers.v1;

public class PlayerController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}