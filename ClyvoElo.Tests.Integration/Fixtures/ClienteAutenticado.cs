using System.Net.Http.Headers;
using System.Net.Http.Json;
using ClyvoElo.Application.DTOs.Auth;

namespace ClyvoElo.Tests.Integration.Fixtures;

public static class ClienteAutenticado
{
    public const string EmailTutor = "gabriel@email.com";
    public const string EmailOutroTutor = "marina@email.com";
    public const string EmailDoutor = "ana.ferreira@clyvovet.com";
    public const string Senha = "clyvo123";

    public static Task<HttpClient> ComoTutorAsync(ClyvoEloApiFactory factory, string email = EmailTutor)
        => CriarAsync(factory, "/api/Auth/login/tutor", email);

    public static Task<HttpClient> ComoDoutorAsync(ClyvoEloApiFactory factory, string email = EmailDoutor)
        => CriarAsync(factory, "/api/Auth/login/doutor", email);

    private static async Task<HttpClient> CriarAsync(ClyvoEloApiFactory factory, string rotaLogin, string email)
    {
        var client = factory.CreateClient();
        var resposta = await client.PostAsJsonAsync(rotaLogin, new LoginRequest { Email = email, Senha = Senha });
        resposta.EnsureSuccessStatusCode();

        var login = await resposta.Content.ReadFromJsonAsync<LoginResponse>();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", login!.Token);
        return client;
    }
}
