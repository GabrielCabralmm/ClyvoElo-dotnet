using ClyvoElo.Application.Common;
using ClyvoElo.Application.DTOs.Pets;
using ClyvoElo.Application.Exceptions;
using ClyvoElo.Application.Mappers;
using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Filtros;
using ClyvoElo.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace ClyvoElo.Application.Services;

public class PetService : IPetService
{
    private readonly IPetRepository _petRepository;
    private readonly ITutorRepository _tutorRepository;
    private readonly ILogger<PetService> _logger;

    public PetService(IPetRepository petRepository, ITutorRepository tutorRepository, ILogger<PetService> logger)
    {
        _petRepository = petRepository;
        _tutorRepository = tutorRepository;
        _logger = logger;
    }

    public async Task<ResultadoPaginado<PetResponse>> ListarAsync(PetFiltro filtro, CancellationToken cancellationToken = default)
    {
        var resultado = await _petRepository.ListarAsync(filtro, cancellationToken);
        return resultado.Mapear(pet => pet.ParaResposta());
    }

    public Task<ResultadoPaginado<PetResponse>> ListarDoTutorAsync(int idTutor, PetFiltro filtro, CancellationToken cancellationToken = default)
    {
        filtro.IdTutor = idTutor;
        return ListarAsync(filtro, cancellationToken);
    }

    public async Task<PetResponse> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var pet = await ObterPetAsync(id, cancellationToken);
        return pet.ParaResposta();
    }

    public async Task<PetResponse> CriarAsync(CriarPetRequest request, UsuarioLogado usuario, CancellationToken cancellationToken = default)
    {
        var idTutor = request.IdTutor ?? usuario.Id;
        if (idTutor != usuario.Id)
        {
            throw new AcessoNegadoException("Um tutor só pode cadastrar pets na própria conta.");
        }

        var tutor = await _tutorRepository.BuscarPorIdAsync(idTutor, cancellationToken)
            ?? throw new RecursoNaoEncontradoException($"Tutor {idTutor} não encontrado.");

        var pet = Pet.Criar(tutor.IdTutor, request.Nome, request.Especie, request.Raca, request.DataNascimento, request.PesoKg);
        await _petRepository.AdicionarAsync(pet, cancellationToken);
        pet.Tutor = tutor;

        _logger.LogInformation("Pet {IdPet} cadastrado para o tutor {IdTutor}", pet.IdPet, pet.IdTutor);
        return pet.ParaResposta();
    }

    public async Task AtualizarPesoAsync(int id, decimal novoPeso, UsuarioLogado usuario, CancellationToken cancellationToken = default)
    {
        var pet = await ObterPetDoTutorAsync(id, usuario, cancellationToken);

        pet.AtualizarPeso(novoPeso);
        await _petRepository.AtualizarAsync(pet, cancellationToken);

        _logger.LogInformation("Peso do pet {IdPet} atualizado para {PesoKg} kg", pet.IdPet, novoPeso);
    }

    public async Task RemoverAsync(int id, UsuarioLogado usuario, CancellationToken cancellationToken = default)
    {
        var pet = await ObterPetDoTutorAsync(id, usuario, cancellationToken);

        await _petRepository.RemoverAsync(pet, cancellationToken);
        _logger.LogInformation("Pet {IdPet} removido", id);
    }

    private async Task<Pet> ObterPetAsync(int id, CancellationToken cancellationToken)
    {
        return await _petRepository.BuscarPorIdAsync(id, cancellationToken)
            ?? throw new RecursoNaoEncontradoException($"Pet {id} não encontrado.");
    }

    private async Task<Pet> ObterPetDoTutorAsync(int id, UsuarioLogado usuario, CancellationToken cancellationToken)
    {
        var pet = await ObterPetAsync(id, cancellationToken);
        if (!pet.PertenceAoTutor(usuario.Id))
        {
            throw new AcessoNegadoException("Esse pet pertence a outro tutor.");
        }
        return pet;
    }
}
