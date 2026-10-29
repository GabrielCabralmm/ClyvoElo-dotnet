using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Filtros;
using ClyvoElo.Domain.Repositories;
using ClyvoElo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClyvoElo.Infrastructure.Repositories;

public class DispositivoIotRepository : IDispositivoIotRepository
{
    private readonly ClyvoEloContext _context;

    public DispositivoIotRepository(ClyvoEloContext context)
    {
        _context = context;
    }

    public Task<DispositivoIot?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.DispositivosIot.FirstOrDefaultAsync(d => d.IdDispositivo == id, cancellationToken);
    }

    public Task<ResultadoPaginado<DispositivoIot>> ListarAsync(DispositivoIotFiltro filtro, CancellationToken cancellationToken = default)
    {
        var query = _context.DispositivosIot.AsNoTracking().AsQueryable();

        if (filtro.IdPet.HasValue)
        {
            query = query.Where(d => d.IdPet == filtro.IdPet.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtro.TipoDispositivo))
        {
            var tipo = filtro.TipoDispositivo.Trim().ToUpper();
            query = query.Where(d => d.TipoDispositivo.ToUpper() == tipo);
        }

        query = filtro.OrdenarPor?.ToLowerInvariant() switch
        {
            "tipo" => query.OrdenarPor(d => d.TipoDispositivo, filtro.EhDescendente()),
            "datainstalacao" => query.OrdenarPor(d => d.DataInstalacao, filtro.EhDescendente()),
            _ => query.OrdenarPor(d => d.IdDispositivo, filtro.EhDescendente()),
        };

        return query.PaginarAsync(filtro, cancellationToken);
    }
}
