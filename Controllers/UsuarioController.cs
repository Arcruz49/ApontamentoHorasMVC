using Microsoft.AspNetCore.Mvc;
using ApontamentoHoras.Models;
using Microsoft.AspNetCore.Identity;
using ApontamentoHoras.Models.Resources;
using ApontamentoHoras.Data;
using ApontamentoHoras.Utils;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace ApontamentoHoras.Controllers;

public class UsuarioController : Controller
{

    private readonly Context db;
    public Util util = new Util();
    public UsuarioController(Context context)
    {
        db = context;
    }

    [Authorize]
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

            var passwordHasher = new PasswordHasher<Usuario>();
            var result = passwordHasher.VerifyHashedPassword(usuario, usuario.password, password);

            if (result != PasswordVerificationResult.Success)
                return Json(new { success = false, message = "Senha incorreta" });

            // Criar claims do usuário
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, usuario.fullName),
                new Claim(ClaimTypes.NameIdentifier, usuario.name),
                new Claim("UserId", usuario.id.ToString()),
                new Claim("Admin", (usuario.admin ?? false).ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12)
            };

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
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
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



    [Authorize]
    [HttpGet]
    public JsonResult GetUserPermission()
    {
        try
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            var adminClaim = User.FindFirst("Admin")?.Value;

            if (userIdClaim == null)
                return Json(new { success = false, message = "Usuário não autenticado" });

            bool isAdmin = adminClaim == "True";

            return Json(new
            {
                success = true,
                userId = userIdClaim,
                adminClaim = adminClaim,
                isAdmin = isAdmin
            });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = $"Erro: {ex.Message}"
            });
        }
    }


    [Authorize]
    [HttpGet]
    public JsonResult GetUsers()
    {
        try
        {
            var users = (from a in db.usuario
                         select new ResourceUsuario
                         {
                             id = a.id,
                             name = a.name,
                             fullName = a.fullName,
                             admin = a.admin,
                             id_cargo = a.id_cargo,
                             id_turno = a.id_turno,
                             dtCreation = a.dtCreation,
                         }).ToList();

            return Json(new { success = true, message = "", data = users });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = util.ErrorMessage(ex) });
        }
    }
    
    [Authorize]
    [HttpPost]
    public JsonResult DeleteUser(int id = 0)
    {
        try
        {
            if (id == 0) return Json(new { success = false, message = "Id não identificado" });

            var user = db.usuario.Where(a => a.id == id).FirstOrDefault();

            if (user == null) return Json(new { success = false, message = "Usuário não encontrado" });

            var apontamentos = db.apontamento.Where(a => a.id_usuario == id).ToList();

            db.RemoveRange(apontamentos);
            db.Remove(user);
            db.SaveChanges();

            return Json(new { success = true, message = "Usuário deletado" });
        }
        catch(Exception ex)
        {
            return Json(new { success = false, message = util.ErrorMessage(ex) });
        }
    }


}
