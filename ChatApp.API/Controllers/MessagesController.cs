using Microsoft.AspNetCore.Mvc;

namespace ChatApp.API.Controllers;
public class MessagesController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
