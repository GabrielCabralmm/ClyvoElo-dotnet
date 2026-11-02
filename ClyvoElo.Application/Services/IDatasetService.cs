namespace ClyvoElo.Application.Services;

public interface IDatasetService
{
    Task<string> ListarPetsJsonAsync(string? especie, CancellationToken cancellationToken = default);
    Task<string> ExportarAsync(CancellationToken cancellationToken = default);
}
