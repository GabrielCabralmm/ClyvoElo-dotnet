using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Filtros;

namespace ClyvoElo.Domain.Repositories;

public interface IDoutorRepository
{
    Task<Doutor?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Doutor?> BuscarPorEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<ResultadoPaginado<Doutor>> ListarAsync(DoutorFiltro filtro, CancellationToken cancellationToken = default);
}
