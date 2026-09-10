namespace ClyvoElo.Api.Models;

public class LeituraSensor
{
    public int IdLeitura { get; set; }
    public int IdDispositivo { get; set; }
    public string TipoMedida { get; set; } = string.Empty;
    public decimal ValorMedido { get; set; }
    public DateTime DataHoraLeitura { get; set; }

    public DispositivoIot? Dispositivo { get; set; }
}
