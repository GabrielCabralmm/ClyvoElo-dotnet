using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using ClyvoElo.Api.Models;

namespace ClyvoElo.Tests.Integration;

[Collection("ClyvoEloApi")]
public class AuthTests
{
    private readonly HttpClient _client;

    public AuthTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Login_CredenciaisValidas_RetornaTokenComPapel()
    {
        var request = new LoginRequest { Email = "gabriel@email.com", Senha = "clyvo123" };

        var resposta = await _client.PostAsJsonAsync("/api/Auth/login/tutor", request);

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var corpo = await resposta.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.NotNull(corpo);
        Assert.False(string.IsNullOrEmpty(corpo!.Token));
        Assert.Equal("Tutor", corpo.Papel);
    }

    [Fact]
    public async Task Login_CredenciaisInvalidas_RetornaUnauthorized()
    {
        var request = new LoginRequest { Email = "gabriel@email.com", Senha = "senhaerrada" };

        var resposta = await _client.PostAsJsonAsync("/api/Auth/login/tutor", request);

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }
}
