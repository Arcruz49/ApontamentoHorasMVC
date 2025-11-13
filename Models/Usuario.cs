using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApontamentoHoras.Models
{
    public class Usuario
    {
        public int id { get; set; }
        public string name { get; set; }
        public string password { get; set; }
        public string fullName { get; set; }
        public DateTime dtCreation { get; set; }
        public bool? admin {get; set;}
        

        [ForeignKey("cargo")]
        [Column("id_cargo")]
        public int? id_cargo { get; set; }
        public Cargo cargo { get; set; }

        [ForeignKey("turno")]
        [Column("id_turno")]
        public int? id_turno { get; set; }
        public Turno turno { get; set; }

        public List<Apontamento> apontamentos { get; set; } = new();
    }
}
