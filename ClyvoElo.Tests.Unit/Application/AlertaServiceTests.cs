using ClyvoElo.Application.DTOs.Alertas;
using ClyvoElo.Application.Exceptions;
using ClyvoElo.Application.Interfaces;
using ClyvoElo.Application.Services;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ClyvoElo.Tests.Unit.Application;

public class AlertaServiceTests
{
    private readonly Mock<IProcedimentosOracle> _procedimentos = new();
    private readonly AlertaService _service;

    public AlertaServiceTests()
    {
        _service = new AlertaService(_procedimentos.Object, NullLogger<AlertaService>.Instance);
    }

    [Fact]
    public async Task RegistrarAsync_ChamaAProcedureEDevolveOIdGerado()
    {
        var request = new RegistrarAlertaRequest { IdPet = 2, Origem = " ambiental ", Prioridade = "alta", SomaFatores = 26, QtdFatores = 3, Mensagem = "Bebedouro vazio" };
        _procedimentos
            .Setup(p => p.RegistrarAlertaAsync(2, " ambiental ", "alta", 26, 3, "Bebedouro vazio", It.IsAny<CancellationToken>()))
            .ReturnsAsync(15);

        var alerta = await _service.RegistrarAsync(request);

        Assert.Equal(15, alerta.IdAlerta);
        Assert.Equal("AMBIENTAL", alerta.Origem);
        Assert.Equal("ALTA", alerta.Prioridade);
    }

    [Fact]
    public async Task RegistrarAsync_PetInexistenteNoOracle_PropagaRecursoNaoEncontrado()
    {
        var request = new RegistrarAlertaRequest { IdPet = 999, Origem = "CLINICO", Prioridade = "ALTA", SomaFatores = 8, QtdFatores = 1, Mensagem = "Teste" };
        _procedimentos
            .Setup(p => p.RegistrarAlertaAsync(999, It.IsAny<string>(), It.IsAny<string>(), It.IsAny<decimal>(), It.IsAny<decimal>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new RecursoNaoEncontradoException("Alerta: pet 999 nao encontrado"));

        await Assert.ThrowsAsync<RecursoNaoEncontradoException>(() => _service.RegistrarAsync(request));
    }

    [Fact]
    public async Task AtualizarStatusAsync_RepassaIdEStatusParaAProcedure()
    {
        await _service.AtualizarStatusAsync(3, "ENVIADO");

        _procedimentos.Verify(p => p.AtualizarStatusAlertaAsync(3, "ENVIADO", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ExcluirAsync_RepassaOIdParaAProcedure()
    {
        await _service.ExcluirAsync(4);

        _procedimentos.Verify(p => p.ExcluirAlertaAsync(4, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GerarRelatorioAsync_DevolveOTextoMontadoNoOracle()
    {
        _procedimentos.Setup(p => p.GerarRelatorioAlertasAsync("CLINICO", It.IsAny<CancellationToken>())).ReturnsAsync("Total Geral 22.40");

        var relatorio = await _service.GerarRelatorioAsync("CLINICO");

        Assert.Equal("Total Geral 22.40", relatorio);
    }
}
