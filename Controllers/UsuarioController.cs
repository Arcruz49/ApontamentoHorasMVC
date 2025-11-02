using Microsoft.AspNetCore.Mvc;
using ApontamentoHoras.Models;
using Microsoft.AspNetCore.Identity;
using ApontamentoHoras.Models.Resources;
using ApontamentoHoras.Data;
using ApontamentoHoras.Utils;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;

namespace ApontamentoHoras.Controllers;

public class UsuarioController : Controller
{

    private readonly Context db;
    public Util util = new Util();
    public UsuarioController(Context context)
    {
        db = context;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    public async Task<JsonResult> Login(string username = "", string password = "")
    {
        try
        {
            if (string.IsNullOrEmpty(username))
                return Json(new { success = false, message = "Usuário inválido" });

            if (string.IsNullOrEmpty(password))
                return Json(new { success = false, message = "Senha inválida" });

            var usuario = db.usuario.FirstOrDefault(a => a.name == username);
            if (usuario == null)
                return Json(new { success = false, message = "Usuário não encontrado" });

            // Verifica o hash da senha
            var passwordHasher = new PasswordHasher<Usuario>();
            var result = passwordHasher.VerifyHashedPassword(usuario, usuario.password, password);

            if (result != PasswordVerificationResult.Success)
                return Json(new { success = false, message = "Senha incorreta" });

            // Criar claims do usuário
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.name),
                new Claim("UserId", usuario.id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true, // persistente após fechar o navegador
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(1)
            };

            // Efetiva o login
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties
            );

            return Json(new { success = true, message = "Login realizado com sucesso" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = util.ErrorMessage(ex) });
        }
    }

    [HttpGet]
    public JsonResult Test()
    {
        RetornoGenerico<Usuario> retorno = util.CreateUserHelper();

        if (retorno.success)
        {
            db.usuario.Add(retorno.data);
            db.SaveChanges();
            return Json(new { success = true, message = retorno.message });

        }

        return Json(new { success = false, message = retorno.message });

    }


    [HttpGet]
    public JsonResult Test2()
    {

        int loggedUserId = util.GetIdLoggedUser(User);

        return Json(new { success = false, message = loggedUserId });

    }


}
