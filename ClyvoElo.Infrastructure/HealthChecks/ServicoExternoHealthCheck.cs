using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ClyvoElo.Infrastructure.HealthChecks;

public class ServicoExternoHealthCheck : IHealthCheck
{
    public const string NomeCliente = "ServicoExterno";

    private readonly IHttpClientFactory _httpClientFactory;
    private readonly string _urlVerificacao;

    public ServicoExternoHealthCheck(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _urlVerificacao = configuration["ServicoExterno:UrlVerificacao"] ?? "https://www.google.com";
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            var client = _httpClientFactory.CreateClient(NomeCliente);
            var resposta = await client.GetAsync(_urlVerificacao, cancellationToken);

            return resposta.IsSuccessStatusCode
                ? HealthCheckResult.Healthy("Conectividade com serviços externos está OK.")
                : HealthCheckResult.Degraded($"Serviço externo respondeu com status {(int)resposta.StatusCode}.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Sem conectividade com serviços externos.", ex);
        }
    }
}
