using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ClyvoElo.Application.DTOs.Alertas;
using ClyvoElo.Tests.Integration.Fixtures;

namespace ClyvoElo.Tests.Integration.Endpoints;

[Collection(ClyvoEloApiCollection.Nome)]
public class ProcedimentosEndpointsTests
{
    private readonly ClyvoEloApiFactory _factory;

    public ProcedimentosEndpointsTests(ClyvoEloApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task PostAlerta_DoutorLogado_RetornaCreatedComIdGerado()
    {
        var client = await ClienteAutenticado.ComoDoutorAsync(_factory);
        var request = new RegistrarAlertaRequest { IdPet = 2, Origem = "ambiental", Prioridade = "alta", SomaFatores = 26, QtdFatores = 3, Mensagem = "Bebedouro do Bento vazio" };

        var resposta = await client.PostAsJsonAsync("/api/Alertas", request);

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        var alerta = await resposta.Content.ReadFromJsonAsync<AlertaRegistradoResponse>();
        Assert.True(alerta!.IdAlerta > 0);
        Assert.Equal("AMBIENTAL", alerta.Origem);
    }

    [Fact]
    public async Task PostAlerta_QuantidadeDeFatoresZero_RetornaBadRequestComMensagemDoOracle()
    {
        var client = await ClienteAutenticado.ComoDoutorAsync(_factory);
        var request = new RegistrarAlertaRequest { IdPet = 1, Origem = "CLINICO", Prioridade = "ALTA", SomaFatores = 9, QtdFatores = 0, Mensagem = "Teste" };

        var resposta = await client.PostAsJsonAsync("/api/Alertas", request);

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        var problema = await resposta.Content.ReadFromJsonAsync<JsonElement>();
        Assert.Contains("quantidade de fatores", problema.GetProperty("detail").GetString());
    }

    [Fact]
    public async Task PostAlerta_PetInexistente_RetornaNotFound()
    {
        var client = await ClienteAutenticado.ComoDoutorAsync(_factory);
        var request = new RegistrarAlertaRequest { IdPet = 999, Origem = "CLINICO", Prioridade = "ALTA", SomaFatores = 8, QtdFatores = 1, Mensagem = "Teste" };

        var resposta = await client.PostAsJsonAsync("/api/Alertas", request);

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task PostAlerta_ComTokenDeTutor_RetornaForbidden()
    {
        var client = await ClienteAutenticado.ComoTutorAsync(_factory);
        var request = new RegistrarAlertaRequest { IdPet = 1, Origem = "CLINICO", Prioridade = "ALTA", SomaFatores = 8, QtdFatores = 1, Mensagem = "Teste" };

        var resposta = await client.PostAsJsonAsync("/api/Alertas", request);

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Fact]
    public async Task PutStatusEDeleteAlerta_AlertaExistente_RetornaNoContent()
    {
        var client = await ClienteAutenticado.ComoDoutorAsync(_factory);

        var atualizacao = await client.PutAsJsonAsync("/api/Alertas/1/status", "RESOLVIDO");
        var exclusao = await client.DeleteAsync("/api/Alertas/2");

        Assert.Equal(HttpStatusCode.NoContent, atualizacao.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, exclusao.StatusCode);
    }

    [Fact]
    public async Task DeleteAlerta_Inexistente_RetornaNotFound()
    {
        var client = await ClienteAutenticado.ComoDoutorAsync(_factory);

        var resposta = await client.DeleteAsync("/api/Alertas/12345");

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task GetRelatorio_SemToken_RetornaUnauthorized()
    {
        var resposta = await _factory.CreateClient().GetAsync("/api/Alertas/relatorio");

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task GetRelatorio_DoutorLogado_RetornaTextoComTotalGeral()
    {
        var client = await ClienteAutenticado.ComoDoutorAsync(_factory);

        var resposta = await client.GetAsync("/api/Alertas/relatorio");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Equal("text/plain", resposta.Content.Headers.ContentType!.MediaType);
        Assert.Contains("Total Geral", await resposta.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task GetPetsJson_SemToken_RetornaJsonMontadoNoOracle()
    {
        var resposta = await _factory.CreateClient().GetAsync("/api/Dataset/pets");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var pets = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        Assert.Equal("Toby", pets.RootElement[0].GetProperty("nome_pet").GetString());
    }

    [Fact]
    public async Task GetExportar_DoutorLogado_DevolveArquivoJsonParaOMongo()
    {
        var client = await ClienteAutenticado.ComoDoutorAsync(_factory);

        var resposta = await client.GetAsync("/api/Dataset/exportar");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        Assert.Equal("application/json", resposta.Content.Headers.ContentType!.MediaType);
        Assert.Equal("2TDSPO_2TDSPX_2026_Dataset_Gabriel_Joao.json", resposta.Content.Headers.ContentDisposition!.FileNameStar ?? resposta.Content.Headers.ContentDisposition.FileName);
        var documentos = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        Assert.Equal(1, documentos.RootElement[0].GetProperty("_id").GetInt32());
    }
}
