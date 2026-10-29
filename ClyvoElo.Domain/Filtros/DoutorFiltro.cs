using ClyvoElo.Domain.Common;

namespace ClyvoElo.Domain.Filtros;

public class DoutorFiltro : FiltroPaginado
{
    public string? Nome { get; set; }
    public int? IdClinica { get; set; }
}
