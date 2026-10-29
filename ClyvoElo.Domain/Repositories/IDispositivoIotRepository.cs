using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Filtros;

namespace ClyvoElo.Domain.Repositories;

public interface IDispositivoIotRepository
{
    Task<DispositivoIot?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ResultadoPaginado<DispositivoIot>> ListarAsync(DispositivoIotFiltro filtro, CancellationToken cancellationToken = default);
}
