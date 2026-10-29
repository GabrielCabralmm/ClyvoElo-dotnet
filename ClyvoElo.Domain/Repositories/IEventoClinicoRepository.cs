using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Filtros;

namespace ClyvoElo.Domain.Repositories;

public interface IEventoClinicoRepository
{
    Task<EventoClinico?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ResultadoPaginado<EventoClinico>> ListarAsync(EventoClinicoFiltro filtro, CancellationToken cancellationToken = default);
    Task AdicionarAsync(EventoClinico evento, CancellationToken cancellationToken = default);
    Task AtualizarAsync(EventoClinico evento, CancellationToken cancellationToken = default);
    Task RemoverAsync(EventoClinico evento, CancellationToken cancellationToken = default);
}
