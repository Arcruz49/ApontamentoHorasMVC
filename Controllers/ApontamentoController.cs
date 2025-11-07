using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using ApontamentoHoras.Models;
using ApontamentoHoras.Data;
using ApontamentoHoras.Utils;
using Microsoft.AspNetCore.Authorization;

namespace ApontamentoHoras.Controllers;

public class ApontamentoController : Controller
{

    private readonly Context db;
    public Util util = new Util();

    public ApontamentoController(Context context)
    {
        db = context;
    }

    [Authorize]
    public IActionResult Index()
    {
        return View();
    }


    [Authorize]
    [HttpPost]
    public JsonResult CreateApontamento()
    {
        try
        {
            var usuario = util.GetIdLoggedUser(User);
            if (usuario == 0) return Json(new { success = false, message = "Usuário não logado]" }); 

            var dataHora = DateTime.Now;

            Apontamento apontamento = new Apontamento()
            {
                dtApontamento = DateTime.Now,
                id_usuario = util.GetIdLoggedUser(User)
            };

            db.apontamento.Add(apontamento);

            db.SaveChanges();

            return Json(new { success = true, message = "Ponto registrado com sucesso!" });

        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = util.ErrorMessage(ex) });
        }
    }

    [Authorize]
    [HttpGet]    
    public JsonResult GetLastApontamento()
    {
        try
        {
            var usuario = util.GetIdLoggedUser(User);
            if (usuario == 0) return Json(new { success = false, message = "Usuário não logado]" });

            var apontamento = db.apontamento.Where(a => a.id_usuario == usuario).OrderByDescending(a => a.dtApontamento).Select(a => a.dtApontamento).FirstOrDefault();

            return Json(new { success = true, data = apontamento });

        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = util.ErrorMessage(ex) });
        }
    }


}
