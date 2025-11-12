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
    [Authorize]
    public JsonResult GetApontamentosAnteriores(int dias = 30)
    {
        try
        {
            var usuario = util.GetIdLoggedUser(User);
            
            if (usuario == 0) return Json(new { success = false, message = "Usuário não logado" });


            var hoje = DateTime.Now.Date;

            var apontamentos = db.apontamento
                .Where(a => a.id_usuario == usuario)
                .Where(a => a.dtApontamento < hoje)
                .OrderBy(a => a.dtApontamento)
                .ToList();

            if (!apontamentos.Any())
                return Json(new { success = false, message = "Nenhum apontamento encontrado" });

            // Agrupa por data (somente dia, sem hora)
            var agrupadoPorDia = apontamentos
                .GroupBy(a => a.dtApontamento.Date)
                .OrderByDescending(g => g.Key).Take(dias);

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

                string status = tempoTotal >= jornadaEsperada ? "Concluído" : "Ocorrência";
                
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


    [HttpGet]
    [Authorize]
    public JsonResult GetSaldoHoras()
    {
        try
        {
            var usuario = util.GetIdLoggedUser(User);

            if (usuario == 0) 
                return Json(new { success = false, message = "Usuário não logado" });

            TimeSpan saldoTotal = new TimeSpan();

            var hoje = DateTime.Now.Date;

            var apontamentos = db.apontamento
                .Where(a => a.id_usuario == usuario)
                .Where(a => a.dtApontamento < hoje)
                .OrderBy(a => a.dtApontamento)
                .ToList();

            if (!apontamentos.Any())
                return Json(new { success = false, message = "Nenhum apontamento encontrado" });

            // Agrupa por data (somente dia, sem hora)
            var agrupadoPorDia = apontamentos
                .GroupBy(a => a.dtApontamento.Date)
                .OrderByDescending(g => g.Key);


            foreach (var grupo in agrupadoPorDia)
            {
                var lista = grupo.OrderBy(a => a.dtApontamento).ToList();
                TimeSpan tempoTotal = TimeSpan.Zero;

                // Percorre as batidas de 2 em 2 (entrada → saída)
                for (int i = 0; i < lista.Count - 1; i += 2)
                {
                    DateTime entrada = lista[i].dtApontamento;
                    DateTime saida = lista[i + 1].dtApontamento;

                    if (saida > entrada)
                        tempoTotal += (saida - entrada);
                }

                // Calcula saldo diário
                TimeSpan jornadaEsperada = TimeSpan.FromHours(8);
                TimeSpan saldo = tempoTotal - jornadaEsperada;

                saldoTotal = saldoTotal.Add(saldo);
            }

            // Formatação e sinal
            bool positive = saldoTotal.TotalMinutes >= 0;
            var horas = (int)Math.Floor(Math.Abs(saldoTotal.TotalHours));
            var minutos = Math.Abs(saldoTotal.Minutes);
            string saldoFormatado = $"{horas:D2}:{minutos:D2}";

            return Json(new 
            { 
                success = true, 
                data = saldoFormatado, 
                positive = positive 
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = util.ErrorMessage(ex) });
        }
    }

    [HttpGet]
    [Authorize]
    // função de teste e debug
    public JsonResult GetSaldoHorasDetalhado()
    {
        try
        {
            var usuario = util.GetIdLoggedUser(User);
            if (usuario == 0) return Json(new { success = false, message = "Usuário não logado" });

            var hoje = DateTime.Now.Date;

            var apontamentos = db.apontamento
                .Where(a => a.id_usuario == usuario)
                .Where(a => a.dtApontamento < hoje)
                .OrderBy(a => a.dtApontamento)
                .ToList();


            if (!apontamentos.Any())
                return Json(new { success = false, message = "Nenhum apontamento encontrado" });

            TimeSpan saldoTotal = TimeSpan.Zero;
            TimeSpan totalPositivo = TimeSpan.Zero;
            TimeSpan totalNegativo = TimeSpan.Zero;

            var agrupadoPorDia = apontamentos
                .GroupBy(a => a.dtApontamento.Date)
                .OrderByDescending(g => g.Key);

            var detalhes = new List<object>();

            foreach (var grupo in agrupadoPorDia)
            {
                var lista = grupo.OrderBy(a => a.dtApontamento).ToList();
                TimeSpan tempoTotal = TimeSpan.Zero;

                for (int i = 0; i < lista.Count - 1; i += 2)
                {
                    DateTime entrada = lista[i].dtApontamento;
                    DateTime saida = lista[i + 1].dtApontamento;
                    if (saida > entrada) tempoTotal += (saida - entrada);
                }

                TimeSpan jornada = TimeSpan.FromHours(8);
                TimeSpan saldoDia = tempoTotal - jornada;

                // acumula positivo/negativo separadamente
                if (saldoDia >= TimeSpan.Zero) totalPositivo = totalPositivo.Add(saldoDia);
                else totalNegativo = totalNegativo.Add(saldoDia); // negativo

                saldoTotal = saldoTotal.Add(saldoDia);

                detalhes.Add(new
                {
                    data = grupo.Key.ToString("yyyy-MM-dd"),
                    tempo = tempoTotal.ToString(@"hh\:mm"),
                    saldoDia = (saldoDia < TimeSpan.Zero ? "-" : "+") + $"{Math.Abs((int)saldoDia.TotalHours):D2}:{Math.Abs(saldoDia.Minutes):D2}"
                });
            }

            // formato para totals maiores que 24h (mostra horas totais)
            string FormatTotal(TimeSpan ts)
                => $"{(ts < TimeSpan.Zero ? "-" : "")}{Math.Abs((long)ts.TotalHours)}:{Math.Abs(ts.Minutes):D2}";

            return Json(new
            {
                success = true,
                data = new
                {
                    saldoLiquido = FormatTotal(saldoTotal),
                    totalExtras = FormatTotal(totalPositivo),
                    totalOcorrencias = FormatTotal(totalNegativo),
                    detalhesPorDia = detalhes
                }
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = util.ErrorMessage(ex) });
        }
    }

    [Authorize]
    [HttpGet]
    public JsonResult GetStatusTrabalho()
    {
        try
        {
            var usuario = util.GetIdLoggedUser(User);
            if (usuario == 0) return Json(new { success = false, message = "Usuário não logado" });

            var hoje = DateTime.Now.Date;

            var apontamentos = db.apontamento
                .Where(a => a.id_usuario == usuario && a.dtApontamento.Date == hoje)
                .OrderBy(a => a.dtApontamento)
                .ToList();

            if (apontamentos.Count == 0) return Json(new { success = true, message = "" });

            if (apontamentos.Count == 1) return Json(new { success = true, message = "Trabalhando", horasTrabalhadas = 0, horasTurno = 8 });
            
            TimeSpan horasTotais = TimeSpan.Zero;
            for (int i = 0; i < apontamentos.Count; i += 2)
            {
                var entrada = apontamentos[i].dtApontamento;
                var saida = apontamentos[i + 1].dtApontamento;

                if (saida > entrada) horasTotais += (saida - entrada);
            }

            if (apontamentos.Count % 2 == 1) return Json(new { success = true, message = "Trabalhando", horasTrabalhadas = 0, horasTurno = 8 });

            if (horasTotais.TotalHours >= 8) return Json(new { success = true, message = "Concluído", horasTrabalhadas = horasTotais, horasTurno = 8 });

            else return Json(new { success = true, message = "Trabalhando", horasTrabalhadas = horasTotais, horasTurno = 8 });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = util.ErrorMessage(ex) });
        }
    }



}
