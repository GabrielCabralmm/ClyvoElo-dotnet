using System.Security.Claims;
using ClyvoElo.Api.Controllers;
using ClyvoElo.Api.Hateoas;
using ClyvoElo.Application.Common;
using ClyvoElo.Application.DTOs.Pets;
using ClyvoElo.Application.Services;
using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Filtros;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Moq;

namespace ClyvoElo.Tests.Unit.Api;

public class PetsControllerTests
{
    private readonly Mock<IPetService> _petService = new();
    private readonly PetsController _controller;

    public PetsControllerTests()
    {
        var url = new Mock<IUrlHelper>();
        url.Setup(u => u.RouteUrl(It.IsAny<UrlRouteContext>()))
            .Returns<UrlRouteContext>(contexto => $"/{contexto.RouteName}");

        var usuario = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim("id", "1"),
            new Claim(ClaimTypes.Role, Papeis.Tutor),
        }, "Teste"));

        var httpContext = new DefaultHttpContext { User = usuario };
        httpContext.Request.Path = "/api/Pets";
        httpContext.Request.QueryString = new QueryString("?especie=GATO&pagina=1");

        _controller = new PetsController(_petService.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            Url = url.Object,
        };
    }

    [Fact]
    public async Task BuscarPorId_PetExiste_RetornaOkComLinksHateoas()
    {
        _petService.Setup(s => s.BuscarPorIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PetResponse { IdPet = 1, Nome = "Toby", Especie = "CACHORRO" });

        var resultado = await _controller.BuscarPorId(1, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(resultado.Result);
        var pet = Assert.IsType<PetResponse>(ok.Value);
        Assert.Contains(pet.Links, l => l.Rel == "self" && l.Metodo == "GET");
        Assert.Contains(pet.Links, l => l.Rel == "remover" && l.Metodo == "DELETE");
    }

    [Fact]
    public async Task Listar_MaisDeUmaPagina_RetornaLinksDeNavegacaoMantendoFiltros()
    {
        var pagina = new ResultadoPaginado<PetResponse>(new[] { new PetResponse { IdPet = 1 } }, 1, 1, 3);
        _petService.Setup(s => s.ListarAsync(It.IsAny<PetFiltro>(), It.IsAny<CancellationToken>())).ReturnsAsync(pagina);

        var resultado = await _controller.Listar(new PetFiltro { Pagina = 1, TamanhoPagina = 1 }, CancellationToken.None);

        var ok = Assert.IsType<OkObjectResult>(resultado.Result);
        var resposta = Assert.IsType<RespostaPaginada<PetResponse>>(ok.Value);
        Assert.Equal(3, resposta.TotalPaginas);
        var proxima = Assert.Single(resposta.Links, l => l.Rel == "next");
        Assert.Contains("pagina=2", proxima.Href);
        Assert.Contains("especie=GATO", proxima.Href);
        Assert.DoesNotContain(resposta.Links, l => l.Rel == "prev");
    }

    [Fact]
    public async Task Criar_TutorLogado_RetornaCreatedAtRoute()
    {
        var request = new CriarPetRequest { Nome = "Bidu", Especie = "CACHORRO" };
        _petService.Setup(s => s.CriarAsync(request, It.Is<UsuarioLogado>(u => u.Id == 1), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PetResponse { IdPet = 7, IdTutor = 1, Nome = "Bidu", Especie = "CACHORRO" });

        var resultado = await _controller.Criar(request, CancellationToken.None);

        var criado = Assert.IsType<CreatedAtRouteResult>(resultado.Result);
        Assert.Equal("BuscarPetPorId", criado.RouteName);
        Assert.Equal(7, criado.RouteValues!["id"]);
    }

    [Fact]
    public async Task AtualizarPeso_ServicoConcluiu_RetornaNoContent()
    {
        var resultado = await _controller.AtualizarPeso(1, 12.5m, CancellationToken.None);

        Assert.IsType<NoContentResult>(resultado);
        _petService.Verify(s => s.AtualizarPesoAsync(1, 12.5m, It.Is<UsuarioLogado>(u => u.EhTutor), It.IsAny<CancellationToken>()), Times.Once);
    }
}
