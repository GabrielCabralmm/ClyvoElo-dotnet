using ClyvoElo.Application.Common;

namespace ClyvoElo.Application.DTOs.Pets;

public class PetResponse : RecursoHateoas
{
    public int IdPet { get; set; }
    public int IdTutor { get; set; }
    public string? NomeTutor { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public string? Raca { get; set; }
    public DateTime? DataNascimento { get; set; }
    public decimal? PesoKg { get; set; }
}
