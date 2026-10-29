using ClyvoElo.Application.Common;

namespace ClyvoElo.Application.DTOs.Doutores;

public class DoutorResponse : RecursoHateoas
{
    public int IdDoutor { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Crmv { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public int IdClinica { get; set; }
    public string? NomeClinica { get; set; }
}
