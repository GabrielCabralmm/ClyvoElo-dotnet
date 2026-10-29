using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ClyvoElo.Application.DTOs.LeiturasSensor;
using ClyvoElo.Tests.Integration.Fixtures;

namespace ClyvoElo.Tests.Integration.Endpoints;

[Collection(ClyvoEloApiCollection.Nome)]
public class ConsultasEndpointsTests
{
    private readonly HttpClient _client;

    public ConsultasEndpointsTests(ClyvoEloApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetDoutores_FiltroPorNome_RetornaDoutorSemExporSenha()
    {
        var resposta = await _client.GetAsync("/api/Doutores?nome=ana");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var corpo = await resposta.Content.ReadAsStringAsync();
        using var json = JsonDocument.Parse(corpo);
        var doutor = Assert.Single(json.RootElement.GetProperty("itens").EnumerateArray());
        Assert.Equal("Dra. Ana Ferreira", doutor.GetProperty("nome").GetString());
        Assert.DoesNotContain("senhaHash", corpo, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task GetDoutorPorId_DoutorExistente_RetornaLinkParaOsEventosDele()
    {
        var lista = await _client.GetFromJsonAsync<JsonElement>("/api/Doutores");
        var idDoutor = lista.GetProperty("itens")[0].GetProperty("idDoutor").GetInt32();

        var doutor = await _client.GetFromJsonAsync<JsonElement>($"/api/Doutores/{idDoutor}");

        var linkEventos = doutor.GetProperty("links").EnumerateArray().Single(l => l.GetProperty("rel").GetString() == "eventos_clinicos");
        Assert.Contains($"idDoutor={idDoutor}", linkEventos.GetProperty("href").GetString());
    }

    [Fact]
    public async Task GetDispositivosDoPet_PetComPotes_RetornaDispositivosOrdenadosPorTipo()
    {
        var pets = await _client.GetFromJsonAsync<JsonElement>("/api/Pets?nome=toby");
        var idPet = pets.GetProperty("itens")[0].GetProperty("idPet").GetInt32();

        var resposta = await _client.GetFromJsonAsync<JsonElement>($"/api/DispositivosIot/pet/{idPet}?ordenarPor=tipo");

        var tipos = resposta.GetProperty("itens").EnumerateArray().Select(d => d.GetProperty("tipoDispositivo").GetString()).ToList();
        Assert.Equal(new[] { "POTE_AGUA", "POTE_RACAO" }, tipos);
    }

    [Fact]
    public async Task GetDispositivoPorId_Inexistente_RetornaNotFound()
    {
        var resposta = await _client.GetAsync("/api/DispositivosIot/99999");

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task PostLeitura_DispositivoExistente_RetornaCreatedEApareceNaListagemDoDispositivo()
    {
        var dispositivos = await _client.GetFromJsonAsync<JsonElement>("/api/DispositivosIot?tipoDispositivo=pote_agua");
        var idDispositivo = dispositivos.GetProperty("itens")[0].GetProperty("idDispositivo").GetInt32();
        var request = new RegistrarLeituraRequest { IdDispositivo = idDispositivo, TipoMedida = "AGUA_ML", ValorMedido = 777 };

        var resposta = await _client.PostAsJsonAsync("/api/LeiturasSensor", request);
        var leituras = await _client.GetFromJsonAsync<JsonElement>($"/api/LeiturasSensor/dispositivo/{idDispositivo}");

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        Assert.Equal(777, leituras.GetProperty("itens")[0].GetProperty("valorMedido").GetDecimal());
    }

    [Fact]
    public async Task PostLeitura_DispositivoInexistente_RetornaNotFound()
    {
        var request = new RegistrarLeituraRequest { IdDispositivo = 99999, TipoMedida = "AGUA_ML", ValorMedido = 10 };

        var resposta = await _client.PostAsJsonAsync("/api/LeiturasSensor", request);

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
    }

    [Fact]
    public async Task GetLeiturasDoPet_FiltroPorTipoMedida_RetornaSomenteOTipoPedido()
    {
        var pets = await _client.GetFromJsonAsync<JsonElement>("/api/Pets?nome=toby");
        var idPet = pets.GetProperty("itens")[0].GetProperty("idPet").GetInt32();

        var resposta = await _client.GetFromJsonAsync<JsonElement>($"/api/LeiturasSensor/pet/{idPet}?tipoMedida=racao_gramas");

        var tipos = resposta.GetProperty("itens").EnumerateArray().Select(l => l.GetProperty("tipoMedida").GetString()).ToList();
        Assert.NotEmpty(tipos);
        Assert.All(tipos, tipo => Assert.Equal("RACAO_GRAMAS", tipo));
    }
}
