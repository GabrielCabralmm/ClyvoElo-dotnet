namespace ClyvoElo.Application.DTOs.Alertas;

public class AlertaRegistradoResponse
{
    public int IdAlerta { get; set; }
    public int IdPet { get; set; }
    public string Origem { get; set; } = string.Empty;
    public string Prioridade { get; set; } = string.Empty;
    public string Mensagem { get; set; } = string.Empty;
}
