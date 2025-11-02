namespace ApontamentoHoras.Models.Resources;

public class RetornoGenerico<T>
{
    public bool success { get; set; }
    public string message { get; set; }
    public T data { get; set; }
    
}
