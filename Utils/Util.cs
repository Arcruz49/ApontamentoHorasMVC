using ApontamentoHoras.Models;
using ApontamentoHoras.Data;
using ApontamentoHoras.Models.Resources;
using Microsoft.AspNetCore.Identity;
using System.Data.Common;
using System.Security.Claims;

namespace ApontamentoHoras.Utils;

public class Util
{

    public string ErrorMessage(Exception ex)
    {
        var message = $"Erro: {ex.Message}";
        if (ex.InnerException != null) message += $"\nInner Exception: {ex.InnerException}";
        if (ex.StackTrace != null) message += $"\nStack Trace:\n{ex.StackTrace}";

        return message;
    }


    public RetornoGenerico<Usuario> CreateUserHelper(string user = "arthur.cruz", string password = "123")
    {
        try
        {

            var usuario = new Usuario { name = user };

            var passwordHasher = new PasswordHasher<Usuario>();
            usuario.password = passwordHasher.HashPassword(usuario, password);


            return new RetornoGenerico<Usuario> { success = true, message = "Usuário criado com hash corretamente.", data = usuario };
        }
        catch (Exception ex)
        {
            return new RetornoGenerico<Usuario> { success = false, message = this.ErrorMessage(ex) };
        }
    }
    
    public int GetIdLoggedUser(ClaimsPrincipal user)
    {
        var userIdClaim = user.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value;

        if (userIdClaim != null)
        {
            int userId = int.Parse(userIdClaim);
            return userId;
        }
        return 0;
    }


}
