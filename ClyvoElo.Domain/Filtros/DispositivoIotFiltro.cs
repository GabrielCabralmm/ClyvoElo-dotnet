using ClyvoElo.Domain.Common;

namespace ClyvoElo.Domain.Filtros;

public class DispositivoIotFiltro : FiltroPaginado
{
    public int? IdPet { get; set; }
    public string? TipoDispositivo { get; set; }
}
