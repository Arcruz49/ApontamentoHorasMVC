namespace ApontamentoHoras.Models.Resources;

public class ResourceUsuario
{
    public int id { get; set; }
    public string name { get; set; }
    public string fullName { get; set; }
    public bool? admin { get; set; }
    public int? id_cargo { get; set; }
    public string nmCargo { get; set; }
    public int? id_turno { get; set; }
    public DateTime dtCreation { get; set; }

}