using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ClyvoElo.Tests.Integration.Fakes;

public class HealthCheckSempreSaudavel : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("Simulado no ambiente de testes."));
    }
}
