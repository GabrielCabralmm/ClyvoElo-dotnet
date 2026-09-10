namespace ClyvoElo.Api.Models;

public class CriarEventoClinicoRequest
{
    public int IdPet { get; set; }
    public int IdDoutor { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataEvento { get; set; }
    public string Status { get; set; } = "PENDENTE";
}
