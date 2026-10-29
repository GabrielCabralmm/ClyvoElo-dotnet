using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Filtros;
using ClyvoElo.Domain.Repositories;
using ClyvoElo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ClyvoElo.Infrastructure.Repositories;

public class LeituraSensorRepository : ILeituraSensorRepository
{
    private readonly ClyvoEloContext _context;

    public LeituraSensorRepository(ClyvoEloContext context)
    {
        _context = context;
    }

    public Task<LeituraSensor?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return _context.LeiturasSensor.FirstOrDefaultAsync(l => l.IdLeitura == id, cancellationToken);
    }

    public Task<ResultadoPaginado<LeituraSensor>> ListarAsync(LeituraSensorFiltro filtro, CancellationToken cancellationToken = default)
    {
        var query = _context.LeiturasSensor.AsNoTracking().AsQueryable();

        if (filtro.IdDispositivo.HasValue)
        {
            query = query.Where(l => l.IdDispositivo == filtro.IdDispositivo.Value);
        }

        if (filtro.IdPet.HasValue)
        {
            query = query.Where(l => l.Dispositivo != null && l.Dispositivo.IdPet == filtro.IdPet.Value);
        }

        if (!string.IsNullOrWhiteSpace(filtro.TipoMedida))
        {
            var tipo = filtro.TipoMedida.Trim().ToUpper();
            query = query.Where(l => l.TipoMedida.ToUpper() == tipo);
        }

        if (filtro.DataInicio.HasValue)
        {
            query = query.Where(l => l.DataHoraLeitura >= filtro.DataInicio.Value);
        }

        if (filtro.DataFim.HasValue)
        {
            query = query.Where(l => l.DataHoraLeitura <= filtro.DataFim.Value);
        }

        query = filtro.OrdenarPor?.ToLowerInvariant() switch
        {
            "valor" => query.OrdenarPor(l => l.ValorMedido, filtro.EhDescendente()),
            "tipo" => query.OrdenarPor(l => l.TipoMedida, filtro.EhDescendente()),
            "data" => query.OrdenarPor(l => l.DataHoraLeitura, filtro.EhDescendente()),
            _ => query.OrderByDescending(l => l.DataHoraLeitura),
        };

        return query.PaginarAsync(filtro, cancellationToken);
    }

    public async Task AdicionarAsync(LeituraSensor leitura, CancellationToken cancellationToken = default)
    {
        _context.LeiturasSensor.Add(leitura);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
