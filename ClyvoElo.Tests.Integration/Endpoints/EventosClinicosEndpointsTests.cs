using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ClyvoElo.Application.DTOs.EventosClinicos;
using ClyvoElo.Tests.Integration.Fixtures;

namespace ClyvoElo.Tests.Integration.Endpoints;

[Collection(ClyvoEloApiCollection.Nome)]
public class EventosClinicosEndpointsTests
{
    private readonly ClyvoEloApiFactory _factory;

    public EventosClinicosEndpointsTests(ClyvoEloApiFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetEventos_FiltroPorStatus_RetornaSomenteOStatusPedido()
    {
        var client = _factory.CreateClient();

        var resposta = await client.GetFromJsonAsync<JsonElement>("/api/EventosClinicos?status=pendente");

        var status = resposta.GetProperty("itens").EnumerateArray().Select(e => e.GetProperty("status").GetString()).ToList();
        Assert.NotEmpty(status);
        Assert.All(status, s => Assert.Equal("PENDENTE", s));
    }

    [Fact]
    public async Task PostEvento_DoutorLogado_RetornaCreated()
    {
        var client = await ClienteAutenticado.ComoDoutorAsync(_factory);
        var pets = await client.GetFromJsonAsync<JsonElement>("/api/Pets");
        var idPet = pets.GetProperty("itens")[0].GetProperty("idPet").GetInt32();
        var request = new CriarEventoClinicoRequest { IdPet = idPet, TipoEvento = "medicacao", Descricao = "Antibiótico 7 dias", DataEvento = DateTime.Today };

        var resposta = await client.PostAsJsonAsync("/api/EventosClinicos", request);

        Assert.Equal(HttpStatusCode.Created, resposta.StatusCode);
        var evento = await resposta.Content.ReadFromJsonAsync<EventoClinicoResponse>();
        Assert.Equal("MEDICACAO", evento!.TipoEvento);
        Assert.Equal("PENDENTE", evento.Status);
    }

    [Fact]
    public async Task PostEvento_TipoInvalido_RetornaBadRequest()
    {
        var client = await ClienteAutenticado.ComoDoutorAsync(_factory);
        var pets = await client.GetFromJsonAsync<JsonElement>("/api/Pets");
        var idPet = pets.GetProperty("itens")[0].GetProperty("idPet").GetInt32();
        var request = new CriarEventoClinicoRequest { IdPet = idPet, TipoEvento = "BANHO_E_TOSA", DataEvento = DateTime.Today };

        var resposta = await client.PostAsJsonAsync("/api/EventosClinicos", request);

        Assert.Equal(HttpStatusCode.BadRequest, resposta.StatusCode);
    }

    [Fact]
    public async Task PostEvento_ComTokenDeTutor_RetornaForbidden()
    {
        var client = await ClienteAutenticado.ComoTutorAsync(_factory);
        var request = new CriarEventoClinicoRequest { IdPet = 1, TipoEvento = "VACINA", DataEvento = DateTime.Today };

        var resposta = await client.PostAsJsonAsync("/api/EventosClinicos", request);

        Assert.Equal(HttpStatusCode.Forbidden, resposta.StatusCode);
    }
}
