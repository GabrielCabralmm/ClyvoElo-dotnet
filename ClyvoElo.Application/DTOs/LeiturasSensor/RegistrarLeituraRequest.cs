using System.ComponentModel.DataAnnotations;

namespace ClyvoElo.Application.DTOs.LeiturasSensor;

public class RegistrarLeituraRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Informe o dispositivo que fez a leitura.")]
    public int IdDispositivo { get; set; }

    [Required(ErrorMessage = "O tipo de medida é obrigatório.")]
    [StringLength(50)]
    public string TipoMedida { get; set; } = string.Empty;

    public decimal ValorMedido { get; set; }

    public DateTime? DataHoraLeitura { get; set; }
}
