using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using ClyvoElo.Api.Models;

namespace ClyvoElo.Tests.Integration;

[Collection("ClyvoEloApi")]
public class PetsTests
{
    private readonly HttpClient _client;

    public PetsTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPets_SemAutenticacao_RetornaOk()
    {
        var resposta = await _client.GetAsync("/api/Pets");

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
    }

    [Fact]
    public async Task PostPets_SemToken_RetornaUnauthorized()
    {
        var novoPet = new CriarPetRequest { IdTutor = 1, Nome = "Bidu", Especie = "CACHORRO" };

        var resposta = await _client.PostAsJsonAsync("/api/Pets", novoPet);

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task PostPets_ComTokenTutor_RetornaCreated()
    {
        var loginRequest = new LoginRequest { Email = "gabriel@email.com", Senha = "clyvo123" };
        var loginResposta = await _client.PostAsJsonAsync("/api/Auth/login/tutor", loginRequest);
        var loginCorpo = await loginResposta.Content.ReadFromJsonAsync<LoginResponse>();

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginCorpo!.Token);
        var novoPet = new CriarPetRequest { IdTutor = 1, Nome = "Bidu", Especie = "CACHORRO" };

        var resposta = await _client.PostAsJsonAsync("/api/Pets", novoPet);

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
    }
}
