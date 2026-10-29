using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Filtros;
using ClyvoElo.Domain.Repositories;
using ClyvoElo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClyvoElo.Infrastructure.Repositories;

public class EventoClinicoRepository : IEventoClinicoRepository
{
    private readonly ClyvoEloContext _context;

    public EventoClinicoRepository(ClyvoEloContext context)
    {
        _context = context;
    }

    public Task<EventoClinico?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.EventosClinicos
            .Include(e => e.Pet)
            .Include(e => e.Doutor)
            .FirstOrDefaultAsync(e => e.IdEvento == id, cancellationToken);
    }

    public Task<ResultadoPaginado<EventoClinico>> ListarAsync(EventoClinicoFiltro filtro, CancellationToken cancellationToken = default)
    {
        var query = _context.EventosClinicos
            .AsNoTracking()
            .Include(e => e.Pet)
            .Include(e => e.Doutor)
            .AsQueryable();

        if (filtro.IdPet.HasValue)
        {
            query = query.Where(e => e.IdPet == filtro.IdPet.Value);
        }

        if (filtro.IdDoutor.HasValue)
        {
            query = query.Where(e => e.IdDoutor == filtro.IdDoutor.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtro.TipoEvento))
        {
            var tipo = filtro.TipoEvento.Trim().ToUpperInvariant();
            query = query.Where(e => e.TipoEvento == tipo);
        }

        if (!string.IsNullOrWhiteSpace(filtro.Status))
        {
            var status = filtro.Status.Trim().ToUpperInvariant();
            query = query.Where(e => e.Status == status);
        }

        if (filtro.DataInicio.HasValue)
        {
            query = query.Where(e => e.DataEvento >= filtro.DataInicio.Value);
        }

        if (filtro.DataFim.HasValue)
        {
            query = query.Where(e => e.DataEvento <= filtro.DataFim.Value);
        }

        query = filtro.OrdenarPor?.ToLowerInvariant() switch
        {
            "tipo" => query.OrdenarPor(e => e.TipoEvento, filtro.EhDescendente()),
            "status" => query.OrdenarPor(e => e.Status, filtro.EhDescendente()),
            "id" => query.OrdenarPor(e => e.IdEvento, filtro.EhDescendente()),
            "data" => query.OrdenarPor(e => e.DataEvento, filtro.EhDescendente()),
            _ => query.OrderByDescending(e => e.DataEvento),
        };

        return query.PaginarAsync(filtro, cancellationToken);
    }

    public async Task AdicionarAsync(EventoClinico evento, CancellationToken cancellationToken = default)
    {
        _context.EventosClinicos.Add(evento);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task AtualizarAsync(EventoClinico evento, CancellationToken cancellationToken = default)
    {
        if (_context.Entry(evento).State == EntityState.Detached)
        {
            _context.EventosClinicos.Update(evento);
        }
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoverAsync(EventoClinico evento, CancellationToken cancellationToken = default)
    {
        _context.EventosClinicos.Remove(evento);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
