using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using ClyvoElo.Api.Controllers;
using ClyvoElo.Api.Models;
using ClyvoElo.Api.Services;

namespace ClyvoElo.Tests.Unit;

public class PetsControllerTests
{
    private readonly Mock<IPetService> _petServiceMock;
    private readonly Mock<ILogger<PetsController>> _loggerMock;
    private readonly PetsController _controller;

    public PetsControllerTests()
    {
        _petServiceMock = new Mock<IPetService>();
        _loggerMock = new Mock<ILogger<PetsController>>();
        _controller = new PetsController(_petServiceMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task BuscarPorId_PetExiste_RetornaOk()
    {
        var pet = new Pet { IdPet = 1, Nome = "Toby", Especie = "CACHORRO" };
        _petServiceMock.Setup(s => s.BuscarPorIdAsync(1)).ReturnsAsync(pet);

        var resultado = await _controller.BuscarPorId(1);

        var okResult = Assert.IsType<OkObjectResult>(resultado);
        Assert.Equal(pet, okResult.Value);
    }

    [Fact]
    public async Task BuscarPorId_PetNaoExiste_RetornaNotFound()
    {
        _petServiceMock.Setup(s => s.BuscarPorIdAsync(99)).ReturnsAsync((Pet?)null);

        var resultado = await _controller.BuscarPorId(99);

        Assert.IsType<NotFoundObjectResult>(resultado);
    }

    [Fact]
    public async Task Criar_DadosValidos_RetornaCreated()
    {
        var request = new CriarPetRequest { IdTutor = 1, Nome = "Rex", Especie = "CACHORRO" };
        var petCriado = new Pet { IdPet = 1, IdTutor = 1, Nome = "Rex", Especie = "CACHORRO" };
        _petServiceMock.Setup(s => s.CriarAsync(It.IsAny<Pet>())).ReturnsAsync(petCriado);

        var resultado = await _controller.Criar(request);

        var createdResult = Assert.IsType<CreatedAtActionResult>(resultado);
        Assert.Equal(petCriado, createdResult.Value);
    }

    [Fact]
    public async Task AtualizarPeso_PesoInvalido_RetornaBadRequest()
    {
        _petServiceMock.Setup(s => s.AtualizarPesoAsync(1, -5))
            .ThrowsAsync(new ArgumentException("O peso deve ser maior que zero."));

        var resultado = await _controller.AtualizarPeso(1, -5);

        Assert.IsType<BadRequestObjectResult>(resultado);
    }

    [Fact]
    public async Task AtualizarPeso_PetNaoExiste_RetornaNotFound()
    {
        _petServiceMock.Setup(s => s.AtualizarPesoAsync(99, 10)).ReturnsAsync(false);

        var resultado = await _controller.AtualizarPeso(99, 10);

        Assert.IsType<NotFoundObjectResult>(resultado);
    }
}
