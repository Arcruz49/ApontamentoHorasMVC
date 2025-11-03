
namespace ApontamentoHoras.Models;
public class Usuario
{
    public int id { get; set; }
    public string name { get; set; }
    public string password { get; set; }
    public string fullName { get; set; }
    public DateTime dtCreation { get; set; }
    public List<Apontamento> apontamentos { get; set; }
}
