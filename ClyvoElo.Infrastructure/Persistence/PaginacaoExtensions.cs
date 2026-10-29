using ClyvoElo.Domain.Common;
using Microsoft.EntityFrameworkCore;

namespace ClyvoElo.Infrastructure.Persistence;

public static class PaginacaoExtensions
{
    public static async Task<ResultadoPaginado<T>> PaginarAsync<T>(
        this IQueryable<T> query,
        FiltroPaginado filtro,
        CancellationToken cancellationToken = default)
    {
        var totalItens = await query.CountAsync(cancellationToken);
        var itens = await query
            .Skip(filtro.QuantidadeIgnorada())
            .Take(filtro.TamanhoPagina)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<T>(itens, filtro.Pagina, filtro.TamanhoPagina, totalItens);
    }

    public static IQueryable<T> OrdenarPor<T, TChave>(
        this IQueryable<T> query,
        System.Linq.Expressions.Expression<Func<T, TChave>> chave,
        bool descendente)
    {
        return descendente ? query.OrderByDescending(chave) : query.OrderBy(chave);
    }
}
