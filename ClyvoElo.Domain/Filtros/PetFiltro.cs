using ClyvoElo.Domain.Common;

namespace ClyvoElo.Domain.Filtros;

public class PetFiltro : FiltroPaginado
{
    public string? Nome { get; set; }
    public string? Especie { get; set; }
    public string? Raca { get; set; }
    public int? IdTutor { get; set; }
}
