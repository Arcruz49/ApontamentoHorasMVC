using Microsoft.AspNetCore.Mvc;
using ApontamentoHoras.Models.Resources;
using ApontamentoHoras.Data;
using ApontamentoHoras.Utils;
using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;

namespace ApontamentoHoras.Controllers;

public class CargoController : Controller
{

    private readonly Context db;
    public Util util = new Util();
    public CargoController(Context context)
    {
        db = context;
    }

    [Authorize]
    public IActionResult Index()
    {
        return View();
    }

        

}
