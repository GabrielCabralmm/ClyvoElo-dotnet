using ClyvoElo.Application.Common;
using ClyvoElo.Application.DTOs.EventosClinicos;
using ClyvoElo.Application.Exceptions;
using ClyvoElo.Application.Mappers;
using ClyvoElo.Application.Observabilidade;
using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Filtros;
using ClyvoElo.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace ClyvoElo.Application.Services;

public class EventoClinicoService : IEventoClinicoService
{
    private readonly IEventoClinicoRepository _eventoRepository;
    private readonly IPetRepository _petRepository;
    private readonly IDoutorRepository _doutorRepository;
    private readonly ILogger<EventoClinicoService> _logger;

    public EventoClinicoService(
        IEventoClinicoRepository eventoRepository,
        IPetRepository petRepository,
        IDoutorRepository doutorRepository,
        ILogger<EventoClinicoService> logger)
    {
        _eventoRepository = eventoRepository;
        _petRepository = petRepository;
        _doutorRepository = doutorRepository;
        _logger = logger;
    }

    public async Task<ResultadoPaginado<EventoClinicoResponse>> ListarAsync(EventoClinicoFiltro filtro, CancellationToken cancellationToken = default)
    {
        var resultado = await _eventoRepository.ListarAsync(filtro, cancellationToken);
        return resultado.Mapear(evento => evento.ParaResposta());
    }

    public async Task<EventoClinicoResponse> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var evento = await ObterEventoAsync(id, cancellationToken);
        return evento.ParaResposta();
    }

    public async Task<EventoClinicoResponse> CriarAsync(CriarEventoClinicoRequest request, UsuarioLogado usuario, CancellationToken cancellationToken = default)
    {
        var idDoutor = request.IdDoutor ?? usuario.Id;
        if (idDoutor != usuario.Id)
        {
            throw new AcessoNegadoException("Um doutor só pode registrar eventos clínicos em seu próprio nome.");
        }

        var pet = await _petRepository.BuscarPorIdAsync(request.IdPet, cancellationToken)
            ?? throw new RecursoNaoEncontradoException($"Pet {request.IdPet} não encontrado.");
        var doutor = await _doutorRepository.BuscarPorIdAsync(idDoutor, cancellationToken)
            ?? throw new RecursoNaoEncontradoException($"Doutor {idDoutor} não encontrado.");

        using var activity = ClyvoEloTelemetry.ActivitySource.StartActivity("RegistrarEventoClinico");

        var evento = EventoClinico.Criar(pet.IdPet, doutor.IdDoutor, request.TipoEvento, request.Descricao, request.DataEvento, request.Status);
        activity?.SetTag("evento.tipo", evento.TipoEvento);
        activity?.SetTag("evento.id_pet", evento.IdPet);
        activity?.SetTag("evento.id_doutor", evento.IdDoutor);

        await _eventoRepository.AdicionarAsync(evento, cancellationToken);
        evento.Pet = pet;
        evento.Doutor = doutor;

        ClyvoEloTelemetry.EventosClinicosRegistrados.Add(1, new KeyValuePair<string, object?>("tipo", evento.TipoEvento));
        _logger.LogInformation(
            "Evento {TipoEvento} registrado para o pet {IdPet} pelo doutor {IdDoutor}",
            evento.TipoEvento, evento.IdPet, evento.IdDoutor);

        return evento.ParaResposta();
    }

    public async Task AtualizarStatusAsync(int id, string novoStatus, CancellationToken cancellationToken = default)
    {
        var evento = await ObterEventoAsync(id, cancellationToken);

        evento.AtualizarStatus(novoStatus);
        await _eventoRepository.AtualizarAsync(evento, cancellationToken);

        _logger.LogInformation("Evento {IdEvento} atualizado para o status {Status}", id, evento.Status);
    }

    public async Task RemoverAsync(int id, CancellationToken cancellationToken = default)
    {
        var evento = await ObterEventoAsync(id, cancellationToken);

        await _eventoRepository.RemoverAsync(evento, cancellationToken);
        _logger.LogInformation("Evento {IdEvento} removido", id);
    }

    private async Task<EventoClinico> ObterEventoAsync(int id, CancellationToken cancellationToken)
    {
        return await _eventoRepository.BuscarPorIdAsync(id, cancellationToken)
            ?? throw new RecursoNaoEncontradoException($"Evento clínico {id} não encontrado.");
    }
}
