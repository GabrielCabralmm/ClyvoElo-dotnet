using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ClyvoElo.Api.HealthChecks;

public class ApiSelfHealthCheck : IHealthCheck
{
    public Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(HealthCheckResult.Healthy("API respondendo normalmente."));
    }
}
