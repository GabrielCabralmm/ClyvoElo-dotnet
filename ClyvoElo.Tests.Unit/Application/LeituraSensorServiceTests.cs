using ClyvoElo.Application.DTOs.LeiturasSensor;
using ClyvoElo.Application.Exceptions;
using ClyvoElo.Application.Services;
using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Filtros;
using ClyvoElo.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ClyvoElo.Tests.Unit.Application;

public class LeituraSensorServiceTests
{
    private readonly Mock<ILeituraSensorRepository> _leituraRepository = new();
    private readonly Mock<IDispositivoIotRepository> _dispositivoRepository = new();
    private readonly LeituraSensorService _service;

    public LeituraSensorServiceTests()
    {
        _service = new LeituraSensorService(_leituraRepository.Object, _dispositivoRepository.Object, NullLogger<LeituraSensorService>.Instance);
    }

    [Fact]
    public async Task RegistrarAsync_DispositivoExistente_SalvaLeitura()
    {
        _dispositivoRepository
            .Setup(r => r.BuscarPorIdAsync(3, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DispositivoIot { IdDispositivo = 3, IdPet = 5, TipoDispositivo = "POTE_AGUA" });
        var request = new RegistrarLeituraRequest { IdDispositivo = 3, TipoMedida = "AGUA_ML", ValorMedido = 420 };

        var resposta = await _service.RegistrarAsync(request);

        Assert.Equal(420, resposta.ValorMedido);
        _leituraRepository.Verify(r => r.AdicionarAsync(It.Is<LeituraSensor>(l => l.IdDispositivo == 3), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegistrarAsync_DispositivoInexistente_LancaRecursoNaoEncontradoException()
    {
        _dispositivoRepository.Setup(r => r.BuscarPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((DispositivoIot?)null);
        var request = new RegistrarLeituraRequest { IdDispositivo = 999, TipoMedida = "AGUA_ML", ValorMedido = 10 };

        await Assert.ThrowsAsync<RecursoNaoEncontradoException>(() => _service.RegistrarAsync(request));
    }

    [Fact]
    public async Task BuscarPorIdAsync_LeituraInexistente_LancaRecursoNaoEncontradoException()
    {
        _leituraRepository.Setup(r => r.BuscarPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((LeituraSensor?)null);

        await Assert.ThrowsAsync<RecursoNaoEncontradoException>(() => _service.BuscarPorIdAsync(42));
    }

    [Fact]
    public async Task ListarAsync_RepositorioComLeituras_MapeiaParaResposta()
    {
        var leitura = new LeituraSensor { IdLeitura = 1, IdDispositivo = 3, TipoMedida = "AGUA_ML", ValorMedido = 500, DataHoraLeitura = DateTime.Today };
        _leituraRepository
            .Setup(r => r.ListarAsync(It.IsAny<LeituraSensorFiltro>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultadoPaginado<LeituraSensor>(new[] { leitura }, 1, 10, 1));

        var resultado = await _service.ListarAsync(new LeituraSensorFiltro { IdDispositivo = 3 });

        var item = Assert.Single(resultado.Itens);
        Assert.Equal(500, item.ValorMedido);
        Assert.Equal("AGUA_ML", item.TipoMedida);
    }
}
