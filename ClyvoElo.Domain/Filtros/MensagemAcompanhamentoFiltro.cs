using ClyvoElo.Domain.Common;

namespace ClyvoElo.Domain.Filtros;

public class MensagemAcompanhamentoFiltro : FiltroPaginado
{
    public int? IdPet { get; set; }
    public int? IdTutor { get; set; }
    public int? IdDoutor { get; set; }
    public string? Categoria { get; set; }
    public bool? Lida { get; set; }
}
