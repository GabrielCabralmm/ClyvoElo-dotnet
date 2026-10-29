using ClyvoElo.Application.DTOs.Pets;
using ClyvoElo.Application.Exceptions;
using ClyvoElo.Application.Services;
using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Exceptions;
using ClyvoElo.Domain.Filtros;
using ClyvoElo.Domain.Repositories;
using ClyvoElo.Tests.Unit.Fixtures;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;

namespace ClyvoElo.Tests.Unit.Application;

[Collection("DadosDeTeste")]
public class PetServiceTests
{
    private readonly DadosDeTesteFixture _dados;
    private readonly Mock<IPetRepository> _petRepository = new();
    private readonly Mock<ITutorRepository> _tutorRepository = new();
    private readonly PetService _service;

    public PetServiceTests(DadosDeTesteFixture dados)
    {
        _dados = dados;
        _service = new PetService(_petRepository.Object, _tutorRepository.Object, NullLogger<PetService>.Instance);
    }

    [Fact]
    public async Task CriarAsync_SemIdTutor_UsaTutorDoToken()
    {
        var request = new CriarPetRequest { Nome = "Bidu", Especie = "cachorro" };
        _tutorRepository.Setup(r => r.BuscarPorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(_dados.CriarTutor(1));

        var criado = await _service.CriarAsync(request, _dados.TutorLogado);

        Assert.Equal(1, criado.IdTutor);
        Assert.Equal("CACHORRO", criado.Especie);
        Assert.Equal("Gabriel", criado.NomeTutor);
        _petRepository.Verify(r => r.AdicionarAsync(It.Is<Pet>(p => p.Nome == "Bidu"), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_IdTutorDeOutraConta_LancaAcessoNegadoException()
    {
        var request = new CriarPetRequest { IdTutor = 99, Nome = "Bidu", Especie = "CACHORRO" };

        await Assert.ThrowsAsync<AcessoNegadoException>(() => _service.CriarAsync(request, _dados.TutorLogado));

        _petRepository.Verify(r => r.AdicionarAsync(It.IsAny<Pet>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_TutorInexistente_LancaRecursoNaoEncontradoException()
    {
        var request = new CriarPetRequest { Nome = "Bidu", Especie = "CACHORRO" };
        _tutorRepository.Setup(r => r.BuscarPorIdAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((Tutor?)null);

        await Assert.ThrowsAsync<RecursoNaoEncontradoException>(() => _service.CriarAsync(request, _dados.TutorLogado));
    }

    [Fact]
    public async Task BuscarPorIdAsync_PetNaoExiste_LancaRecursoNaoEncontradoException()
    {
        _petRepository.Setup(r => r.BuscarPorIdAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync((Pet?)null);

        var excecao = await Assert.ThrowsAsync<RecursoNaoEncontradoException>(() => _service.BuscarPorIdAsync(99));

        Assert.Contains("99", excecao.Message);
    }

    [Fact]
    public async Task AtualizarPesoAsync_PesoValido_AtualizaEPersiste()
    {
        var pet = _dados.CriarPet(idTutor: 1);
        _petRepository.Setup(r => r.BuscarPorIdAsync(pet.IdPet, It.IsAny<CancellationToken>())).ReturnsAsync(pet);

        await _service.AtualizarPesoAsync(pet.IdPet, 32.4m, _dados.TutorLogado);

        Assert.Equal(32.4m, pet.PesoKg);
        _petRepository.Verify(r => r.AtualizarAsync(pet, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AtualizarPesoAsync_PesoInvalido_LancaRegraDeNegocioExceptionSemPersistir()
    {
        var pet = _dados.CriarPet(idTutor: 1);
        _petRepository.Setup(r => r.BuscarPorIdAsync(pet.IdPet, It.IsAny<CancellationToken>())).ReturnsAsync(pet);

        await Assert.ThrowsAsync<RegraDeNegocioException>(() => _service.AtualizarPesoAsync(pet.IdPet, -5, _dados.TutorLogado));

        _petRepository.Verify(r => r.AtualizarAsync(It.IsAny<Pet>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarPesoAsync_PetDeOutroTutor_LancaAcessoNegadoException()
    {
        var pet = _dados.CriarPet(idTutor: 1);
        _petRepository.Setup(r => r.BuscarPorIdAsync(pet.IdPet, It.IsAny<CancellationToken>())).ReturnsAsync(pet);

        await Assert.ThrowsAsync<AcessoNegadoException>(() => _service.AtualizarPesoAsync(pet.IdPet, 20, _dados.OutroTutorLogado));
    }

    [Fact]
    public async Task RemoverAsync_PetDoTutor_ChamaRemoverNoRepositorio()
    {
        var pet = _dados.CriarPet(idTutor: 1);
        _petRepository.Setup(r => r.BuscarPorIdAsync(pet.IdPet, It.IsAny<CancellationToken>())).ReturnsAsync(pet);

        await _service.RemoverAsync(pet.IdPet, _dados.TutorLogado);

        _petRepository.Verify(r => r.RemoverAsync(pet, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ListarDoTutorAsync_QualquerFiltro_ForcaIdDoTutorLogado()
    {
        var filtro = new PetFiltro { IdTutor = 99 };
        _petRepository
            .Setup(r => r.ListarAsync(It.IsAny<PetFiltro>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ResultadoPaginado<Pet>(new[] { _dados.CriarPet() }, 1, 10, 1));

        var resultado = await _service.ListarDoTutorAsync(1, filtro);

        Assert.Single(resultado.Itens);
        _petRepository.Verify(r => r.ListarAsync(It.Is<PetFiltro>(f => f.IdTutor == 1), It.IsAny<CancellationToken>()), Times.Once);
    }
}
