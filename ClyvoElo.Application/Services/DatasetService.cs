using ClyvoElo.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClyvoElo.Application.Services;

public class DatasetService : IDatasetService
{
    private readonly IProcedimentosOracle _procedimentos;
    private readonly ILogger<DatasetService> _logger;

    public DatasetService(IProcedimentosOracle procedimentos, ILogger<DatasetService> logger)
    {
        _procedimentos = procedimentos;
        _logger = logger;
    }

    public Task<string> ListarPetsJsonAsync(string? especie, CancellationToken cancellationToken = default)
    {
        return _procedimentos.ListarPetsJsonAsync(especie, cancellationToken);
    }

    public async Task<string> ExportarAsync(CancellationToken cancellationToken = default)
    {
        var json = await _procedimentos.ExportarDatasetAsync(cancellationToken);
        _logger.LogInformation("Dataset exportado do Oracle com {Tamanho} caracteres", json.Length);
        return json;
    }
}
