namespace ClyvoElo.Api.Models;

public class EventoClinico
{
    public int IdEvento { get; set; }
    public int IdPet { get; set; }
    public int IdDoutor { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataEvento { get; set; }
    public string Status { get; set; } = "PENDENTE";

    public Pet? Pet { get; set; }
    public Doutor? Doutor { get; set; }
}
