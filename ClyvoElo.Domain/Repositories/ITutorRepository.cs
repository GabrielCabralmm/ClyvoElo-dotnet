using ClyvoElo.Domain.Entities;

namespace ClyvoElo.Domain.Repositories;

public interface ITutorRepository
{
    Task<Tutor?> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Tutor?> BuscarPorEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<bool> EmailJaCadastradoAsync(string email, CancellationToken cancellationToken = default);
    Task AdicionarAsync(Tutor tutor, CancellationToken cancellationToken = default);
}
