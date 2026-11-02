using System.ComponentModel.DataAnnotations;

namespace ClyvoElo.Application.DTOs.Alertas;

public class RegistrarAlertaRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Informe o pet do alerta.")]
    public int IdPet { get; set; }

    [Required(ErrorMessage = "A origem do alerta é obrigatória.")]
    public string Origem { get; set; } = string.Empty;

    [Required(ErrorMessage = "A prioridade do alerta é obrigatória.")]
    public string Prioridade { get; set; } = string.Empty;

    public decimal SomaFatores { get; set; }

    public decimal QtdFatores { get; set; }

    [Required(ErrorMessage = "A mensagem do alerta é obrigatória.")]
    public string Mensagem { get; set; } = string.Empty;
}
