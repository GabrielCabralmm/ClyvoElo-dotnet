using ClyvoElo.Application.Common;

namespace ClyvoElo.Api.Hateoas;

public class RespostaPaginada<T>
{
    public IReadOnlyList<T> Itens { get; set; } = Array.Empty<T>();
    public int Pagina { get; set; }
    public int TamanhoPagina { get; set; }
    public int TotalItens { get; set; }
    public int TotalPaginas { get; set; }
    public List<LinkDto> Links { get; set; } = new();
}
