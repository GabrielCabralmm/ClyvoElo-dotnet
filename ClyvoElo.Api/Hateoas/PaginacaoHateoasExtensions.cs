using ClyvoElo.Application.Common;
using ClyvoElo.Domain.Common;
using Microsoft.AspNetCore.WebUtilities;

namespace ClyvoElo.Api.Hateoas;

public static class PaginacaoHateoasExtensions
{
    public static RespostaPaginada<T> ParaRespostaPaginada<T>(
        this ResultadoPaginado<T> resultado,
        HttpRequest request,
        Func<T, T> adicionarLinks)
    {
        var resposta = new RespostaPaginada<T>
        {
            Itens = resultado.Itens.Select(adicionarLinks).ToList(),
            Pagina = resultado.Pagina,
            TamanhoPagina = resultado.TamanhoPagina,
            TotalItens = resultado.TotalItens,
            TotalPaginas = resultado.TotalPaginas,
        };

        resposta.Links.Add(new LinkDto(UrlDaPagina(request, resultado.Pagina), "self", "GET"));

        if (resultado.TemPaginaAnterior)
        {
            resposta.Links.Add(new LinkDto(UrlDaPagina(request, 1), "first", "GET"));
            resposta.Links.Add(new LinkDto(UrlDaPagina(request, resultado.Pagina - 1), "prev", "GET"));
        }

        if (resultado.TemProximaPagina)
        {
            resposta.Links.Add(new LinkDto(UrlDaPagina(request, resultado.Pagina + 1), "next", "GET"));
            resposta.Links.Add(new LinkDto(UrlDaPagina(request, resultado.TotalPaginas), "last", "GET"));
        }

        return resposta;
    }

    private static string UrlDaPagina(HttpRequest request, int pagina)
    {
        var parametros = QueryHelpers.ParseQuery(request.QueryString.Value)
            .ToDictionary(p => p.Key, p => (string?)p.Value.ToString(), StringComparer.OrdinalIgnoreCase);

        parametros["pagina"] = pagina.ToString();

        return QueryHelpers.AddQueryString($"{request.PathBase}{request.Path}", parametros);
    }
}
