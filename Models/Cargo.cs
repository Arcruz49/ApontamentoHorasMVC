namespace ApontamentoHoras.Models
{
    public class Cargo
    {
        public int id { get; set; }
        public string name { get; set; }
        public DateTime dtCreation { get; set; }
        public List<Usuario> usuarios { get; set; } = new();
    }
}