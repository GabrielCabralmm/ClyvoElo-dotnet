namespace ClyvoElo.Application.Interfaces;

public interface IProcedimentosOracle
{
    Task<int> RegistrarAlertaAsync(int idPet, string origem, string prioridade, decimal somaFatores, decimal qtdFatores, string mensagem, CancellationToken cancellationToken = default);
    Task AtualizarStatusAlertaAsync(int idAlerta, string status, CancellationToken cancellationToken = default);
    Task ExcluirAlertaAsync(int idAlerta, CancellationToken cancellationToken = default);
    Task<string> GerarRelatorioAlertasAsync(string? origem, CancellationToken cancellationToken = default);
    Task<string> ListarPetsJsonAsync(string? especie, CancellationToken cancellationToken = default);
    Task<string> ExportarDatasetAsync(CancellationToken cancellationToken = default);
}
