using System.ComponentModel.DataAnnotations;

namespace ClyvoElo.Application.DTOs.EventosClinicos;

public class CriarEventoClinicoRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Informe o pet do evento.")]
    public int IdPet { get; set; }

    public int? IdDoutor { get; set; }

    [Required(ErrorMessage = "O tipo do evento é obrigatório.")]
    public string TipoEvento { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Descricao { get; set; }

    [Required(ErrorMessage = "A data do evento é obrigatória.")]
    public DateTime DataEvento { get; set; }

    public string? Status { get; set; }
}
