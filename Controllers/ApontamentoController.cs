using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ApontamentoHoras.Models;

namespace ApontamentoHoras.Controllers;

public class ApontamentoController : Controller
{


    public IActionResult Index()
    {
        return View();
    }




}
