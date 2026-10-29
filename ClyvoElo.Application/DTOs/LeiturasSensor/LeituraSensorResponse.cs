using ClyvoElo.Application.Common;

namespace ClyvoElo.Application.DTOs.LeiturasSensor;

public class LeituraSensorResponse : RecursoHateoas
{
    public int IdLeitura { get; set; }
    public int IdDispositivo { get; set; }
    public string TipoMedida { get; set; } = string.Empty;
    public decimal ValorMedido { get; set; }
    public DateTime DataHoraLeitura { get; set; }
}
