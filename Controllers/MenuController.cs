using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ApontamentoHoras.Data;
using ApontamentoHoras.Utils;
using Microsoft.AspNetCore.Authorization;

namespace ApontamentoHoras.Controllers;

public class MenuController : Controller
{

    private readonly Context db;
    public Util util = new Util();

    public MenuController(Context context)
    {
        db = context;
    }

    [Authorize]

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public JsonResult GetUsuarios()
    {
        try
        {
            var dados = db.usuario.ToList();
            return Json(new { success = true, data = dados });
        }
        catch(Exception ex){

            var message = util.ErrorMessage(ex);
            return Json(new { success = false, message = message });
            
        }
    }

}
