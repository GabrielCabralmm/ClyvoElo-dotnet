using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ClyvoElo.Application.DTOs.Auth;
using ClyvoElo.Tests.Integration.Fixtures;
using Microsoft.AspNetCore.Mvc;

namespace ClyvoElo.Tests.Integration.Endpoints;

[Collection(ClyvoEloApiCollection.Nome)]
public class AuthEndpointsTests
{
    private readonly HttpClient _client;

    public AuthEndpointsTests(ClyvoEloApiFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task LoginTutor_CredenciaisValidas_RetornaTokenComPapelTutor()
    {
        var request = new LoginRequest { Email = ClienteAutenticado.EmailTutor, Senha = ClienteAutenticado.Senha };

        var resposta = await _client.PostAsJsonAsync("/api/Auth/login/tutor", request);

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var corpo = await resposta.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.False(string.IsNullOrEmpty(corpo!.Token));
        Assert.Equal("Tutor", corpo.Papel);
        Assert.True(corpo.ExpiraEm > DateTime.UtcNow);
    }

    [Fact]
    public async Task LoginTutor_SenhaErrada_RetornaUnauthorizedComProblemDetails()
    {
        var request = new LoginRequest { Email = ClienteAutenticado.EmailTutor, Senha = "senhaerrada" };

        var resposta = await _client.PostAsJsonAsync("/api/Auth/login/tutor", request);

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
        var problema = await resposta.Content.ReadFromJsonAsync<ProblemDetails>();
        Assert.Equal(401, problema!.Status);
        Assert.Equal("Email ou senha inválidos.", problema.Detail);
    }

    [Fact]
    public async Task LoginDoutor_CredenciaisValidas_RetornaTokenComPapelDoutor()
    {
        var request = new LoginRequest { Email = ClienteAutenticado.EmailDoutor, Senha = ClienteAutenticado.Senha };

        var resposta = await _client.PostAsJsonAsync("/api/Auth/login/doutor", request);

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var corpo = await resposta.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.Equal("Doutor", corpo!.Papel);
    }

    [Fact]
    public async Task CadastroTutor_EmailJaExistente_RetornaConflict()
    {
        var request = new CadastroTutorRequest { Nome = "Gabriel", Email = ClienteAutenticado.EmailTutor, Senha = "123456" };

        var resposta = await _client.PostAsJsonAsync("/api/Auth/cadastro/tutor", request);

        Assert.Equal(HttpStatusCode.Conflict, resposta.StatusCode);
    }

    [Fact]
    public async Task CadastroTutor_EmailInvalido_RetornaBadRequestComErroDeValidacao()
    {
        var request = new CadastroTutorRequest { Nome = "Teste", Email = "email-sem-arroba", Senha = "123456" };

        var resposta = await _client.PostAsJsonAsync("/api/Auth/cadastro/tutor", request);

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
        using var json = JsonDocument.Parse(await resposta.Content.ReadAsStringAsync());
        Assert.True(json.RootElement.GetProperty("errors").TryGetProperty("Email", out _));
    }

    [Fact]
    public async Task CadastroTutor_DadosValidos_RetornaTokenDoNovoTutor()
    {
        var email = $"novo.{Guid.NewGuid():N}@email.com";
        var request = new CadastroTutorRequest { Nome = "Novo Tutor", Email = email, Senha = "123456" };

        var resposta = await _client.PostAsJsonAsync("/api/Auth/cadastro/tutor", request);

        Assert.Equal(HttpStatusCode.OK, resposta.StatusCode);
        var corpo = await resposta.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.True(corpo!.Id > 0);
    }
}
