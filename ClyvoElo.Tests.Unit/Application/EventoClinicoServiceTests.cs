using ClyvoElo.Application.DTOs.EventosClinicos;
using ClyvoElo.Application.Exceptions;
using ClyvoElo.Application.Services;
using ClyvoElo.Domain.Constantes;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Exceptions;
using ClyvoElo.Domain.Repositories;
using ClyvoElo.Tests.Unit.Fixtures;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ClyvoElo.Tests.Unit.Application;

[Collection("DadosDeTeste")]
public class EventoClinicoServiceTests
{
    private readonly DadosDeTesteFixture _dados;
    private readonly Mock<IEventoClinicoRepository> _eventoRepository = new();
    private readonly Mock<IPetRepository> _petRepository = new();
    private readonly Mock<IDoutorRepository> _doutorRepository = new();
    private readonly EventoClinicoService _service;

    public EventoClinicoServiceTests(DadosDeTesteFixture dados)
    {
        _dados = dados;
        _service = new EventoClinicoService(
            _eventoRepository.Object,
            _petRepository.Object,
            _doutorRepository.Object,
            NullLogger<EventoClinicoService>.Instance);
    }

    [Fact]
    public async Task CriarAsync_DadosValidos_RegistraEventoComDoutorDoToken()
    {
        var pet = _dados.CriarPet();
        var doutor = _dados.CriarDoutor();
        _petRepository.Setup(r => r.BuscarPorIdAsync(pet.IdPet, It.IsAny<CancellationToken>())).ReturnsAsync(pet);
        _doutorRepository.Setup(r => r.BuscarPorIdAsync(doutor.IdDoutor, It.IsAny<CancellationToken>())).ReturnsAsync(doutor);
        var request = new CriarEventoClinicoRequest { IdPet = pet.IdPet, TipoEvento = "vacina", DataEvento = DateTime.Today };

        var evento = await _service.CriarAsync(request, _dados.DoutorLogado);

        Assert.Equal(TiposEventoClinico.Vacina, evento.TipoEvento);
        Assert.Equal(StatusEventoClinico.Pendente, evento.Status);
        Assert.Equal(doutor.IdDoutor, evento.IdDoutor);
        Assert.Equal("Toby", evento.NomePet);
        _eventoRepository.Verify(r => r.AdicionarAsync(It.IsAny<EventoClinico>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_IdDoutorDiferenteDoToken_LancaAcessoNegadoException()
    {
        var request = new CriarEventoClinicoRequest { IdPet = 5, IdDoutor = 77, TipoEvento = "VACINA", DataEvento = DateTime.Today };

        await Assert.ThrowsAsync<AcessoNegadoException>(() => _service.CriarAsync(request, _dados.DoutorLogado));
    }

    [Fact]
    public async Task CriarAsync_PetInexistente_LancaRecursoNaoEncontradoException()
    {
        _petRepository.Setup(r => r.BuscarPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((Pet?)null);
        var request = new CriarEventoClinicoRequest { IdPet = 404, TipoEvento = "VACINA", DataEvento = DateTime.Today };

        await Assert.ThrowsAsync<RecursoNaoEncontradoException>(() => _service.CriarAsync(request, _dados.DoutorLogado));
    }

    [Fact]
    public async Task CriarAsync_TipoInvalido_LancaRegraDeNegocioExceptionSemSalvar()
    {
        _petRepository.Setup(r => r.BuscarPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(_dados.CriarPet());
        _doutorRepository.Setup(r => r.BuscarPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(_dados.CriarDoutor());
        var request = new CriarEventoClinicoRequest { IdPet = 5, TipoEvento = "BANHO", DataEvento = DateTime.Today };

        await Assert.ThrowsAsync<RegraDeNegocioException>(() => _service.CriarAsync(request, _dados.DoutorLogado));

        _eventoRepository.Verify(r => r.AdicionarAsync(It.IsAny<EventoClinico>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarStatusAsync_EventoExistente_PersisteNovoStatus()
    {
        var evento = EventoClinico.Criar(5, 10, TiposEventoClinico.Exame, null, DateTime.Today, null);
        _eventoRepository.Setup(r => r.BuscarPorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(evento);

        await _service.AtualizarStatusAsync(1, "concluido");

        Assert.Equal(StatusEventoClinico.Concluido, evento.Status);
        _eventoRepository.Verify(r => r.AtualizarAsync(evento, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_EventoInexistente_LancaRecursoNaoEncontradoException()
    {
        _eventoRepository.Setup(r => r.BuscarPorIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync((EventoClinico?)null);

        await Assert.ThrowsAsync<RecursoNaoEncontradoException>(() => _service.RemoverAsync(123));
    }
}
