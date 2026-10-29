using ClyvoElo.Domain.Common;

namespace ClyvoElo.Domain.Filtros;

public class EventoClinicoFiltro : FiltroPaginado
{
    public int? IdPet { get; set; }
    public int? IdDoutor { get; set; }
    public string? TipoEvento { get; set; }
    public string? Status { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
}
