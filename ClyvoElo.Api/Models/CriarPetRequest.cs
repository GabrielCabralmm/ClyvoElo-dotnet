namespace ClyvoElo.Api.Models;

public class CriarPetRequest
{
    public int IdTutor { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public string? Raca { get; set; }
    public DateTime? DataNascimento { get; set; }
    public decimal? PesoKg { get; set; }
}
