using System.ComponentModel.DataAnnotations.Schema;

namespace ApontamentoHoras.Models
{
    public class Apontamento
    {
        public int id { get; set; }
        public DateTime dtApontamento { get; set; }

        [ForeignKey("usuario")]
        [Column("id_usuario")] 
        public int id_usuario { get; set; }

        public Usuario usuario { get; set; }
    }
}
