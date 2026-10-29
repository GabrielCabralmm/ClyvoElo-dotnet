using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ClyvoElo.Application.DTOs.Mensagens;
using ClyvoElo.Tests.Integration.Fixtures;

namespace ClyvoElo.Tests.Integration.Endpoints;

[Collection(ClyvoEloApiCollection.Nome)]
public class MensagensEndpointsTests
{
    private readonly ClyvoEloApiFactory _factory;

    public MensagensEndpointsTests(ClyvoEloApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task FluxoMensagem_DoutorEnviaETutorLe_MensagemFicaMarcadaComoLida()
    {
        var doutor = await ClienteAutenticado.ComoDoutorAsync(_factory);
        var tutor = await ClienteAutenticado.ComoTutorAsync(_factory);
        var petsDoTutor = await tutor.GetFromJsonAsync<JsonElement>("/api/Pets/meus");
        var idPet = petsDoTutor.GetProperty("itens")[0].GetProperty("idPet").GetInt32();
        var request = new EnviarMensagemRequest
        {
            IdPet = idPet,
            Categoria = "POS_OPERATORIO",
            Assunto = "Cuidados com os pontos",
            Conteudo = "Manter o colar elizabetano por 10 dias.",
        };

        var envio = await doutor.PostAsJsonAsync("/api/Mensagens", request);
        var enviada = await envio.Content.ReadFromJsonAsync<MensagemResponse>();
        var leitura = await tutor.PatchAsync($"/api/Mensagens/{enviada!.Id}/lida", null);
        var consulta = await tutor.GetFromJsonAsync<MensagemResponse>($"/api/Mensagens/{enviada.Id}");

        Assert.Equal(HttpStatusCode.Created, envio.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, leitura.StatusCode);
        Assert.True(consulta!.Lida);
        Assert.Contains(consulta.Links, l => l.Rel == "marcar_como_lida" && l.Metodo == "PATCH");
    }

    [Fact]
    public async Task GetMensagens_SemToken_RetornaUnauthorized()
    {
        var client = _factory.CreateClient();

        var resposta = await client.GetAsync("/api/Mensagens");

        Assert.Equal(HttpStatusCode.Unauthorized, resposta.StatusCode);
    }

    [Fact]
    public async Task PostMensagem_ComTokenDeTutor_RetornaForbidden()
    {
        var tutor = await ClienteAutenticado.ComoTutorAsync(_factory);
        var request = new EnviarMensagemRequest { IdPet = 1, Categoria = "ORIENTACAO", Assunto = "Oi", Conteudo = "Teste" };

        var resposta = await tutor.PostAsJsonAsync("/api/Mensagens", request);

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }
}
