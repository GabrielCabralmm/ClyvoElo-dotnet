using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ClyvoElo.Api.HealthChecks;

public class ServicoExternoHealthCheck : IHealthCheck
{
    private readonly IHttpClientFactory _httpClientFactory;

    public ServicoExternoHealthCheck(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);
            var resposta = await client.GetAsync("https://www.google.com", cancellationToken);

            return resposta.IsSuccessStatusCode
                ? HealthCheckResult.Healthy("Conectividade com serviços externos está OK.")
                : HealthCheckResult.Degraded("Serviço externo respondeu, mas com status de erro.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Sem conectividade com serviços externos.", ex);
        }
    }
}
