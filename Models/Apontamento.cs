namespace ApontamentoHoras.Models;
public class Apontamento
{
    public int id { get; set; }
    public DateTime dtApontamento { get; set; }
    public int id_usuario { get; set; }
    public Usuario usuario { get; set; }
}