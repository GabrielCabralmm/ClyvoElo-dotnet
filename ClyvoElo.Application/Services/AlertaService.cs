using ClyvoElo.Application.DTOs.Alertas;
using ClyvoElo.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClyvoElo.Application.Services;

public class AlertaService : IAlertaService
{
    private readonly IProcedimentosOracle _procedimentos;
    private readonly ILogger<AlertaService> _logger;

    public AlertaService(IProcedimentosOracle procedimentos, ILogger<AlertaService> logger)
    {
        _procedimentos = procedimentos;
        _logger = logger;
    }

    public async Task<AlertaRegistradoResponse> RegistrarAsync(RegistrarAlertaRequest request, CancellationToken cancellationToken = default)
    {
        var idAlerta = await _procedimentos.RegistrarAlertaAsync(
            request.IdPet, request.Origem, request.Prioridade,
            request.SomaFatores, request.QtdFatores, request.Mensagem, cancellationToken);

        _logger.LogInformation("Alerta {IdAlerta} registrado para o pet {IdPet} via pkg_clyvo_alerta", idAlerta, request.IdPet);

        return new AlertaRegistradoResponse
        {
            IdAlerta = idAlerta,
            IdPet = request.IdPet,
            Origem = request.Origem.Trim().ToUpperInvariant(),
            Prioridade = request.Prioridade.Trim().ToUpperInvariant(),
            Mensagem = request.Mensagem,
        };
    }

    public async Task AtualizarStatusAsync(int idAlerta, string status, CancellationToken cancellationToken = default)
    {
        await _procedimentos.AtualizarStatusAlertaAsync(idAlerta, status, cancellationToken);
        _logger.LogInformation("Status do alerta {IdAlerta} atualizado para {Status}", idAlerta, status);
    }

    public async Task ExcluirAsync(int idAlerta, CancellationToken cancellationToken = default)
    {
        await _procedimentos.ExcluirAlertaAsync(idAlerta, cancellationToken);
        _logger.LogInformation("Alerta {IdAlerta} excluído", idAlerta);
    }

    public Task<string> GerarRelatorioAsync(string? origem, CancellationToken cancellationToken = default)
    {
        return _procedimentos.GerarRelatorioAlertasAsync(origem, cancellationToken);
    }
}
