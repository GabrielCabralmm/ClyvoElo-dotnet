using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Filtros;

namespace ClyvoElo.Domain.Repositories;

public interface IPetRepository
{
    Task<Pet?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ResultadoPaginado<Pet>> ListarAsync(PetFiltro filtro, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Pet pet, CancellationToken cancellationToken = default);
    Task AtualizarAsync(Pet pet, CancellationToken cancellationToken = default);
    Task RemoverAsync(Pet pet, CancellationToken cancellationToken = default);
}
