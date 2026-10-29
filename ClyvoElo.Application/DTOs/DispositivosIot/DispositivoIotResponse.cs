using ClyvoElo.Application.Common;

namespace ClyvoElo.Application.DTOs.DispositivosIot;

public class DispositivoIotResponse : RecursoHateoas
{
    public int IdDispositivo { get; set; }
    public int IdPet { get; set; }
    public string TipoDispositivo { get; set; } = string.Empty;
    public DateTime DataInstalacao { get; set; }
}
