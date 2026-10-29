using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ClyvoElo.Application.DTOs.Pets;
using ClyvoElo.Tests.Integration.Fixtures;
using Microsoft.AspNetCore.Mvc;

namespace ClyvoElo.Tests.Integration.Endpoints;

[Collection(ClyvoEloApiCollection.Nome)]
public class PetsEndpointsTests
{
    private readonly ClyvoEloApiFactory _factory;
    private readonly HttpClient _client;

    public PetsEndpointsTests(ClyvoEloApiFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPets_SemAutenticacao_RetornaPaginaComLinksHateoas()
    {
        var resposta = await _client.GetAsync("/api/Pets?pagina=1&tamanhoPagina=2&ordenarPor=nome");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        using var json = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        var raiz = json.RootElement;
        Assert.Equal(2, raiz.GetProperty("itens").GetArrayLength());
        Assert.True(raiz.GetProperty("totalItens").GetInt32() >= 3);
        var relsDaPagina = raiz.GetProperty("links").EnumerateArray().Select(l => l.GetProperty("rel").GetString()).ToList();
        Assert.Contains("self", relsDaPagina);
        Assert.Contains("next", relsDaPagina);
        var linksDoPrimeiroPet = raiz.GetProperty("itens")[0].GetProperty("links").EnumerateArray().Select(l => l.GetProperty("rel").GetString());
        Assert.Contains("eventos_clinicos", linksDoPrimeiroPet);
    }

    [Fact]
    public async Task GetPets_FiltroPorEspecie_RetornaSomenteDaEspecie()
    {
        var resposta = await _client.GetAsync("/api/Pets?especie=gato");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        using var json = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        var especies = json.RootElement.GetProperty("itens").EnumerateArray().Select(p => p.GetProperty("especie").GetString()).ToList();
        Assert.NotEmpty(especies);
        Assert.All(especies, especie => Assert.Equal("GATO", especie));
    }

    [Fact]
    public async Task GetPetPorId_PetInexistente_RetornaNotFoundComProblemDetails()
    {
        var resposta = await _client.GetAsync("/api/Pets/99999");

        Assert.Equal(HttpStatusCode.NotFound, resposta.StatusCode);
        var problema = await resposta.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal("Recurso não encontrado", problema!.Title);
    }

    [Fact]
    public async Task PostPets_SemToken_RetornaUnauthorized()
    {
        var novoPet = new CriarPetRequest { Nome = "Bidu", Especie = "CACHORRO" };

        var resposta = await _client.PostAsJsonAsync("/api/Pets", novoPet);

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task PostPets_ComTokenDeDoutor_RetornaForbidden()
    {
        var client = await ClienteAutenticado.ComoDoutorAsync(_factory);
        var novoPet = new CriarPetRequest { Nome = "Bidu", Especie = "CACHORRO" };

        var resposta = await client.PostAsJsonAsync("/api/Pets", novoPet);

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }

    [Fact]
    public async Task PostPets_ComTokenTutor_RetornaCreatedComLocation()
    {
        var client = await ClienteAutenticado.ComoTutorAsync(_factory);
        var novoPet = new CriarPetRequest { Nome = "Bidu", Especie = "cachorro", PesoKg = 8.3m };

        var resposta = await client.PostAsJsonAsync("/api/Pets", novoPet);

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        Assert.NotNull(resposta.Headers.Location);
        var criado = await resposta.Content.ReadFromJsonAsync<PetResponse>();
        Assert.Equal("CACHORRO", criado!.Especie);
        Assert.Contains(criado.Links, l => l.Rel == "self");
    }

    [Fact]
    public async Task PutPeso_PesoNegativo_RetornaBadRequest()
    {
        var client = await ClienteAutenticado.ComoTutorAsync(_factory);
        var meusPets = await client.GetFromJsonAsync<JsonElement>("/api/Pets/meus");
        var idPet = meusPets.GetProperty("itens")[0].GetProperty("idPet").GetInt32();

        var resposta = await client.PutAsJsonAsync($"/api/Pets/{idPet}/peso", -3m);

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task PutPeso_PetDeOutroTutor_RetornaForbidden()
    {
        var clientMarina = await ClienteAutenticado.ComoTutorAsync(_factory, ClienteAutenticado.EmailOutroTutor);
        var clientGabriel = await ClienteAutenticado.ComoTutorAsync(_factory);
        var petsDoGabriel = await clientGabriel.GetFromJsonAsync<JsonElement>("/api/Pets/meus");
        var idPetDoGabriel = petsDoGabriel.GetProperty("itens")[0].GetProperty("idPet").GetInt32();

        var resposta = await clientMarina.PutAsJsonAsync($"/api/Pets/{idPetDoGabriel}/peso", 10m);

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }
}
