using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Filtros;

namespace ClyvoElo.Domain.Repositories;

public interface ILeituraSensorRepository
{
    Task<LeituraSensor?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ResultadoPaginado<LeituraSensor>> ListarAsync(LeituraSensorFiltro filtro, CancellationToken cancellationToken = default);
    Task AdicionarAsync(LeituraSensor leitura, CancellationToken cancellationToken = default);
}
