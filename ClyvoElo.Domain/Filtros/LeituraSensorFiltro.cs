using ClyvoElo.Domain.Common;

namespace ClyvoElo.Domain.Filtros;

public class LeituraSensorFiltro : FiltroPaginado
{
    public int? IdPet { get; set; }
    public int? IdDispositivo { get; set; }
    public string? TipoMedida { get; set; }
    public DateTime? DataInicio { get; set; }
    public DateTime? DataFim { get; set; }
}
