using ClyvoElo.Application.DTOs.Alertas;

namespace ClyvoElo.Application.Services;

public interface IAlertaService
{
    Task<AlertaRegistradoResponse> RegistrarAsync(RegistrarAlertaRequest request, CancellationToken cancellationToken = default);
    Task AtualizarStatusAsync(int idAlerta, string status, CancellationToken cancellationToken = default);
    Task ExcluirAsync(int idAlerta, CancellationToken cancellationToken = default);
    Task<string> GerarRelatorioAsync(string? origem, CancellationToken cancellationToken = default);
}
