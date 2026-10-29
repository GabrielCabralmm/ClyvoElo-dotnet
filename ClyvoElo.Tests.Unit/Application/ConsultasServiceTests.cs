using ClyvoElo.Application.Exceptions;
using ClyvoElo.Application.Services;
using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Filtros;
using ClyvoElo.Domain.Repositories;
using Moq;

namespace ClyvoElo.Tests.Unit.Application;

public class ConsultasServiceTests
{
    [Fact]
    public async Task DoutorService_ListarAsync_NaoExpoeSenhaEMapeiaClinica()
    {
        var repository = new Mock<IDoutorRepository>();
        var doutor = new Doutor
        {
            IdDoutor = 1,
            Nome = "Dra. Ana",
            SenhaHash = "hash-secreto",
            Clinica = new Clinica { IdClinica = 1, Nome = "Clyvo Vet Paulista" },
        };
        repository
            .Setup(r => r.ListarAsync(It.IsAny<DoutorFiltro>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultadoPaginado<Doutor>(new[] { doutor }, 1, 10, 1));
        var service = new DoutorService(repository.Object);

        var resultado = await service.ListarAsync(new DoutorFiltro());

        var item = Assert.Single(resultado.Itens);
        Assert.Equal("Clyvo Vet Paulista", item.NomeClinica);
        Assert.Null(item.GetType().GetProperty(nameof(Doutor.SenhaHash)));
    }

    [Fact]
    public async Task DoutorService_BuscarPorIdAsync_Inexistente_LancaRecursoNaoEncontradoException()
    {
        var repository = new Mock<IDoutorRepository>();
        repository.Setup(r => r.BuscarPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((Doutor?)null);
        var service = new DoutorService(repository.Object);

        await Assert.ThrowsAsync<RecursoNaoEncontradoException>(() => service.BuscarPorIdAsync(50));
    }

    [Fact]
    public async Task DispositivoIotService_ListarAsync_RepassaFiltroParaORepositorio()
    {
        var repository = new Mock<IDispositivoIotRepository>();
        repository
            .Setup(r => r.ListarAsync(It.IsAny<DispositivoIotFiltro>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultadoPaginado<DispositivoIot>(Array.Empty<DispositivoIot>(), 1, 10, 0));
        var service = new DispositivoIotService(repository.Object);
        var filtro = new DispositivoIotFiltro { IdPet = 5, TipoDispositivo = "POTE_AGUA" };

        var resultado = await service.ListarAsync(filtro);

        Assert.Empty(resultado.Itens);
        repository.Verify(r => r.ListarAsync(filtro, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DispositivoIotService_BuscarPorIdAsync_Existente_MapeiaDispositivo()
    {
        var repository = new Mock<IDispositivoIotRepository>();
        repository
            .Setup(r => r.BuscarPorIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DispositivoIot { IdDispositivo = 3, IdPet = 5, TipoDispositivo = "POTE_RACAO" });
        var service = new DispositivoIotService(repository.Object);

        var dispositivo = await service.BuscarPorIdAsync(3);

        Assert.Equal(5, dispositivo.IdPet);
        Assert.Equal("POTE_RACAO", dispositivo.TipoDispositivo);
    }

    [Fact]
    public async Task DispositivoIotService_BuscarPorIdAsync_Inexistente_LancaRecursoNaoEncontradoException()
    {
        var repository = new Mock<IDispositivoIotRepository>();
        repository.Setup(r => r.BuscarPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((DispositivoIot?)null);
        var service = new DispositivoIotService(repository.Object);

        await Assert.ThrowsAsync<RecursoNaoEncontradoException>(() => service.BuscarPorIdAsync(77));
    }
}
