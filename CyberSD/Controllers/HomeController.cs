using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using CyberSD.Models;
using Microsoft.AspNetCore.Authorization;

namespace CyberSD.Controllers;
// using Microsoft.AspNetCore.Identity;
[Authorize(Roles = "Gestor_Equipos,Empleado,Administrador")]
public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
