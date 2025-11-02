using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ApontamentoHoras.Models;
using Microsoft.AspNetCore.Authorization;

namespace ApontamentoHoras.Controllers;

public class HomeController : Controller
{


    [Authorize]
    public IActionResult Index()
    {
        return View();
    }




}
