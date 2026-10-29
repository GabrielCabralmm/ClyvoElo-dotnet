using ClyvoElo.Application.Common;
using ClyvoElo.Application.DTOs.Auth;
using ClyvoElo.Application.Exceptions;
using ClyvoElo.Application.Interfaces;
using ClyvoElo.Application.Services;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Repositories;
using ClyvoElo.Tests.Unit.Fixtures;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ClyvoElo.Tests.Unit.Application;

[Collection("DadosDeTeste")]
public class AuthServiceTests
{
    private readonly DadosDeTesteFixture _dados;
    private readonly Mock<ITutorRepository> _tutorRepository = new();
    private readonly Mock<IDoutorRepository> _doutorRepository = new();
    private readonly Mock<IPasswordHasher> _passwordHasher = new();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly AuthService _service;

    public AuthServiceTests(DadosDeTesteFixture dados)
    {
        _dados = dados;
        _tokenService
            .Setup(t => t.Gerar(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new TokenGerado("token-de-teste", DateTime.UtcNow.AddHours(2)));

        _service = new AuthService(
            _tutorRepository.Object,
            _doutorRepository.Object,
            _passwordHasher.Object,
            _tokenService.Object,
            NullLogger<AuthService>.Instance);
    }

    [Fact]
    public async Task LoginTutorAsync_CredenciaisValidas_RetornaTokenComPapelTutor()
    {
        var tutor = _dados.CriarTutor();
        _tutorRepository.Setup(r => r.BuscarPorEmailAsync(tutor.Email, It.IsAny<CancellationToken>())).ReturnsAsync(tutor);
        _passwordHasher.Setup(h => h.Verificar("clyvo123", tutor.SenhaHash)).Returns(true);

        var resposta = await _service.LoginTutorAsync(new LoginRequest { Email = tutor.Email, Senha = "clyvo123" });

        Assert.Equal("token-de-teste", resposta.Token);
        Assert.Equal(Papeis.Tutor, resposta.Papel);
        Assert.Equal(tutor.IdTutor, resposta.Id);
    }

    [Fact]
    public async Task LoginTutorAsync_SenhaErrada_LancaAutenticacaoException()
    {
        var tutor = _dados.CriarTutor();
        _tutorRepository.Setup(r => r.BuscarPorEmailAsync(tutor.Email, It.IsAny<CancellationToken>())).ReturnsAsync(tutor);
        _passwordHasher.Setup(h => h.Verificar(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        await Assert.ThrowsAsync<AutenticacaoException>(
            () => _service.LoginTutorAsync(new LoginRequest { Email = tutor.Email, Senha = "errada" }));

        _tokenService.Verify(t => t.Gerar(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task LoginDoutorAsync_EmailInexistente_LancaAutenticacaoException()
    {
        _doutorRepository.Setup(r => r.BuscarPorEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync((Doutor?)null);

        await Assert.ThrowsAsync<AutenticacaoException>(
            () => _service.LoginDoutorAsync(new LoginRequest { Email = "ninguem@clyvovet.com", Senha = "clyvo123" }));
    }

    [Fact]
    public async Task LoginDoutorAsync_CredenciaisValidas_RetornaTokenComPapelDoutor()
    {
        var doutor = _dados.CriarDoutor();
        _doutorRepository.Setup(r => r.BuscarPorEmailAsync(doutor.Email, It.IsAny<CancellationToken>())).ReturnsAsync(doutor);
        _passwordHasher.Setup(h => h.Verificar("clyvo123", doutor.SenhaHash)).Returns(true);

        var resposta = await _service.LoginDoutorAsync(new LoginRequest { Email = doutor.Email, Senha = "clyvo123" });

        Assert.Equal(Papeis.Doutor, resposta.Papel);
        _tokenService.Verify(t => t.Gerar(doutor.IdDoutor, doutor.Email, Papeis.Doutor), Times.Once);
    }

    [Fact]
    public async Task CadastrarTutorAsync_EmailJaCadastrado_LancaConflitoException()
    {
        _tutorRepository.Setup(r => r.EmailJaCadastradoAsync("gabriel@email.com", It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var request = new CadastroTutorRequest { Nome = "Gabriel", Email = "gabriel@email.com", Senha = "123456" };

        await Assert.ThrowsAsync<ConflitoException>(() => _service.CadastrarTutorAsync(request));

        _tutorRepository.Verify(r => r.AdicionarAsync(It.IsAny<Tutor>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CadastrarTutorAsync_DadosValidos_SalvaSenhaComHash()
    {
        _tutorRepository.Setup(r => r.EmailJaCadastradoAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(false);
        _passwordHasher.Setup(h => h.GerarHash("123456")).Returns("hash-gerado");
        var request = new CadastroTutorRequest { Nome = " Novo Tutor ", Email = "novo@email.com", Senha = "123456" };

        var resposta = await _service.CadastrarTutorAsync(request);

        Assert.Equal("Novo Tutor", resposta.Nome);
        _tutorRepository.Verify(r => r.AdicionarAsync(
            It.Is<Tutor>(t => t.SenhaHash == "hash-gerado" && t.Email == "novo@email.com"),
            It.IsAny<CancellationToken>()), Times.Once);
    }
}
