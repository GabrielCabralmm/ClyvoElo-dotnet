using ClyvoElo.Application.DTOs.Mensagens;
using ClyvoElo.Application.Exceptions;
using ClyvoElo.Application.Services;
using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Constantes;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Filtros;
using ClyvoElo.Domain.Repositories;
using ClyvoElo.Tests.Unit.Fixtures;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ClyvoElo.Tests.Unit.Application;

[Collection("DadosDeTeste")]
public class MensagemAcompanhamentoServiceTests
{
    private readonly DadosDeTesteFixture _dados;
    private readonly Mock<IMensagemAcompanhamentoRepository> _mensagemRepository = new();
    private readonly Mock<IPetRepository> _petRepository = new();
    private readonly Mock<IDoutorRepository> _doutorRepository = new();
    private readonly MensagemAcompanhamentoService _service;

    public MensagemAcompanhamentoServiceTests(DadosDeTesteFixture dados)
    {
        _dados = dados;
        _service = new MensagemAcompanhamentoService(
            _mensagemRepository.Object,
            _petRepository.Object,
            _doutorRepository.Object,
            NullLogger<MensagemAcompanhamentoService>.Instance);
    }

    [Fact]
    public async Task EnviarAsync_DadosValidos_SalvaMensagemParaOTutorDoPet()
    {
        _petRepository.Setup(r => r.BuscarPorIdAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(_dados.CriarPet(5, idTutor: 1));
        _doutorRepository.Setup(r => r.BuscarPorIdAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync(_dados.CriarDoutor(10));
        var request = new EnviarMensagemRequest
        {
            IdPet = 5,
            Categoria = CategoriasMensagem.PosOperatorio,
            Assunto = "Pontos",
            Conteudo = "Evite que o Toby lamba os pontos.",
        };

        var resposta = await _service.EnviarAsync(request, _dados.DoutorLogado);

        Assert.Equal(1, resposta.IdTutor);
        Assert.Equal("Dra. Ana Ferreira", resposta.NomeDoutor);
        _mensagemRepository.Verify(r => r.AdicionarAsync(
            It.Is<MensagemAcompanhamento>(m => m.IdTutor == 1 && m.IdDoutor == 10 && !m.Lida),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListarAsync_UsuarioTutor_FiltraPelasMensagensDoProprioTutor()
    {
        _mensagemRepository
            .Setup(r => r.ListarAsync(It.IsAny<MensagemAcompanhamentoFiltro>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultadoPaginado<MensagemAcompanhamento>(new[] { _dados.CriarMensagem() }, 1, 10, 1));

        await _service.ListarAsync(new MensagemAcompanhamentoFiltro { IdTutor = 2 }, _dados.TutorLogado);

        _mensagemRepository.Verify(r => r.ListarAsync(
            It.Is<MensagemAcompanhamentoFiltro>(f => f.IdTutor == _dados.TutorLogado.Id),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task BuscarPorIdAsync_TutorDeOutroPet_LancaAcessoNegadoException()
    {
        var mensagem = _dados.CriarMensagem(idTutor: 1);
        _mensagemRepository.Setup(r => r.BuscarPorIdAsync(mensagem.Id, It.IsAny<CancellationToken>())).ReturnsAsync(mensagem);

        await Assert.ThrowsAsync<AcessoNegadoException>(() => _service.BuscarPorIdAsync(mensagem.Id, _dados.OutroTutorLogado));
    }

    [Fact]
    public async Task MarcarComoLidaAsync_TutorDestinatario_AtualizaMensagem()
    {
        var mensagem = _dados.CriarMensagem(idTutor: 1);
        _mensagemRepository.Setup(r => r.BuscarPorIdAsync(mensagem.Id, It.IsAny<CancellationToken>())).ReturnsAsync(mensagem);

        await _service.MarcarComoLidaAsync(mensagem.Id, _dados.TutorLogado);

        Assert.True(mensagem.Lida);
        _mensagemRepository.Verify(r => r.AtualizarAsync(mensagem, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_DoutorQueNaoEnviou_LancaAcessoNegadoException()
    {
        var mensagem = _dados.CriarMensagem(idDoutor: 99);
        _mensagemRepository.Setup(r => r.BuscarPorIdAsync(mensagem.Id, It.IsAny<CancellationToken>())).ReturnsAsync(mensagem);

        await Assert.ThrowsAsync<AcessoNegadoException>(() => _service.RemoverAsync(mensagem.Id, _dados.DoutorLogado));

        _mensagemRepository.Verify(r => r.RemoverAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task BuscarPorIdAsync_IdInexistente_LancaRecursoNaoEncontradoException()
    {
        _mensagemRepository.Setup(r => r.BuscarPorIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((MensagemAcompanhamento?)null);

        await Assert.ThrowsAsync<RecursoNaoEncontradoException>(() => _service.BuscarPorIdAsync("naoexiste", _dados.DoutorLogado));
    }
}
