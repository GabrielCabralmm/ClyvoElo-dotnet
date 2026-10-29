namespace ClyvoElo.Domain.Common;

public class ResultadoPaginado<T>
{
    public ResultadoPaginado(IReadOnlyList<T> itens, int pagina, int tamanhoPagina, int totalItens)
    {
        Itens = itens;
        Pagina = pagina;
        TamanhoPagina = tamanhoPagina;
        TotalItens = totalItens;
    }

    public IReadOnlyList<T> Itens { get; }
    public int Pagina { get; }
    public int TamanhoPagina { get; }
    public int TotalItens { get; }

    public int TotalPaginas => TamanhoPagina == 0 ? 0 : (int)Math.Ceiling(TotalItens / (double)TamanhoPagina);
    public bool TemProximaPagina => Pagina < TotalPaginas;
    public bool TemPaginaAnterior => Pagina > 1;

    public ResultadoPaginado<TDestino> Mapear<TDestino>(Func<T, TDestino> conversor)
    {
        return new ResultadoPaginado<TDestino>(Itens.Select(conversor).ToList(), Pagina, TamanhoPagina, TotalItens);
    }
}
