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
            if (usuario == 0) return Json(new { success = false, message = "Usuário não logado" });

            var apontamento = db.apontamento.Where(a => a.id_usuario == usuario).OrderByDescending(a => a.dtApontamento).Select(a => a.dtApontamento).FirstOrDefault();

            return Json(new { success = true, data = apontamento });

        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = util.ErrorMessage(ex) });
        }
    }
    
    [HttpGet]
    public JsonResult GetApontamentosAnteriores()
    {
        try
        {
            var usuario = util.GetIdLoggedUser(User);
            if (usuario == 0)
                return Json(new { success = false, message = "Usuário não logado" });

            var apontamentos = db.apontamento
                .Where(a => a.id_usuario == usuario)
                .OrderBy(a => a.dtApontamento)
                .ToList();

            if (!apontamentos.Any())
                return Json(new { success = false, message = "Nenhum apontamento encontrado" });

            // Agrupa por data (somente dia, sem hora)
            var agrupadoPorDia = apontamentos
                .GroupBy(a => a.dtApontamento.Date)
                .OrderByDescending(g => g.Key);

            List<ResourceApontamento> resultApontamentos = new();

            foreach (var grupo in agrupadoPorDia)
            {
                var lista = grupo.OrderBy(a => a.dtApontamento).ToList();
                TimeSpan tempoTotal = TimeSpan.Zero;

                // Percorre as batidas de 2 em 2 (entrada → saída)
                for (int i = 0; i < lista.Count - 1; i += 2)
                {
                    DateTime entrada = lista[i].dtApontamento;
                    DateTime saida = lista[i + 1].dtApontamento;

                    // Garante que a saída é depois da entrada
                    if (saida > entrada)
                    {
                        tempoTotal += (saida - entrada);
                    }
                }

                // Calcula saldo e status
                TimeSpan jornadaEsperada = TimeSpan.FromHours(8);
                TimeSpan saldo = tempoTotal - jornadaEsperada;

                string status;
                if (tempoTotal == jornadaEsperada)
                    status = "Concluído";
                else if (tempoTotal < jornadaEsperada)
                    status = "Ocorrência";
                else
                    status = "Excedente";

                resultApontamentos.Add(new ResourceApontamento
                {
                    Data = grupo.Key,
                    Tempo = tempoTotal,
                    Saldo = saldo,
                    Status = status
                });
            }

            return Json(new { success = true, data = resultApontamentos });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = util.ErrorMessage(ex) });
        }
    }




}
