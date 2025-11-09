namespace ApontamentoHoras.Models;

public class TurnoWeekday
{
    public int id { get; set; }
    public int turno_id { get; set; }

    public string weekday { get; set; } // sunday, monday, etc.
    public TimeSpan entryTime { get; set; }
    public TimeSpan exitTime { get; set; }

    public Turno Turno { get; set; }
}