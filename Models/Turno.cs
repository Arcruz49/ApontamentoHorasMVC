using System;
using System.Collections.Generic;

namespace ApontamentoHoras.Models
{
    public class Turno
    {
        public int id { get; set; }
        public string name { get; set; }
        public string description { get; set; }
        public DateTime dtCreation { get; set; }

        public List<TurnoWeekday> Weekdays { get; set; }
        public List<Usuario> Usuarios { get; set; }
    }
}
