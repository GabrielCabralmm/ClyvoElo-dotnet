using ClyvoElo.Api.Data;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ClyvoElo.Api.HealthChecks;

public class OracleHealthCheck : IHealthCheck
{
    private readonly ClyvoEloContext _context;

    public OracleHealthCheck(ClyvoEloContext context)
    {
        _context = context;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var conseguiuConectar = await _context.Database.CanConnectAsync(cancellationToken);
            return conseguiuConectar
                ? HealthCheckResult.Healthy("Conexão com o Oracle está OK.")
                : HealthCheckResult.Unhealthy("Não foi possível conectar ao Oracle.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Erro ao verificar o Oracle.", ex);
        }
    }
}
