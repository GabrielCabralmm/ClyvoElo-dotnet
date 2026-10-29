using System.Net;
using System.Text.Json;
using ClyvoElo.Tests.Integration.Fixtures;

namespace ClyvoElo.Tests.Integration.Endpoints;

[Collection(ClyvoEloApiCollection.Nome)]
public class HealthChecksEndpointsTests
{
    private readonly HttpClient _client;

    public HealthChecksEndpointsTests(ClyvoEloApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task HealthLive_ApiNoAr_RetornaSomenteOCheckDaApi()
    {
        var resposta = await _client.GetAsync("/health/live");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var nomes = await LerNomesDosChecksAsync(resposta);
        Assert.Equal(new[] { "api" }, nomes);
    }

    [Fact]
    public async Task HealthReady_DependenciasDisponiveis_RetornaBancosEServicoExternoSemOCheckDaApi()
    {
        var resposta = await _client.GetAsync("/health/ready");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var nomes = await LerNomesDosChecksAsync(resposta);
        Assert.Contains("oracle_db", nomes);
        Assert.Contains("mongodb", nomes);
        Assert.Contains("servico_externo", nomes);
        Assert.DoesNotContain("api", nomes);
    }

    [Fact]
    public async Task Health_TodasAsVerificacoes_RetornaQuatroChecksComStatusHealthy()
    {
        var resposta = await _client.GetAsync("/health");

        using var json = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        Assert.Equal("Healthy", json.RootElement.GetProperty("status").GetString());
        Assert.Equal(4, json.RootElement.GetProperty("checks").GetArrayLength());
    }

    [Fact]
    public async Task QualquerRequisicao_ComCorrelationId_DevolveOMesmoIdNoCabecalho()
    {
        var requisicao = new HttpRequestMessage(HttpMethod.Get, "/health/live");
        requisicao.Headers.Add("X-Correlation-Id", "teste-correlacao-123");

        var resposta = await _client.SendAsync(requisicao);

        Assert.Equal("teste-correlacao-123", resposta.Headers.GetValues("X-Correlation-Id").Single());
    }

    private static async Task<List<string>> LerNomesDosChecksAsync(HttpResponseMessage resposta)
    {
        using var json = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        return json.RootElement.GetProperty("checks").EnumerateArray()
            .Select(c => c.GetProperty("nome").GetString()!)
            .ToList();
    }
}
