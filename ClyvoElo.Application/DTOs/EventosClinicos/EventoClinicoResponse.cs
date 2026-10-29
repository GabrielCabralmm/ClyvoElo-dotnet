using ClyvoElo.Application.Common;

namespace ClyvoElo.Application.DTOs.EventosClinicos;

public class EventoClinicoResponse : RecursoHateoas
{
    public int IdEvento { get; set; }
    public int IdPet { get; set; }
    public string? NomePet { get; set; }
    public int IdDoutor { get; set; }
    public string? NomeDoutor { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataEvento { get; set; }
    public string Status { get; set; } = string.Empty;
}
