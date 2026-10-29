using ClyvoElo.Application.Common;
using ClyvoElo.Application.DTOs.Mensagens;
using ClyvoElo.Application.Exceptions;
using ClyvoElo.Application.Mappers;
using ClyvoElo.Application.Observabilidade;
using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Filtros;
using ClyvoElo.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace ClyvoElo.Application.Services;

public class MensagemAcompanhamentoService : IMensagemAcompanhamentoService
{
    private readonly IMensagemAcompanhamentoRepository _mensagemRepository;
    private readonly IPetRepository _petRepository;
    private readonly IDoutorRepository _doutorRepository;
    private readonly ILogger<MensagemAcompanhamentoService> _logger;

    public MensagemAcompanhamentoService(
        IMensagemAcompanhamentoRepository mensagemRepository,
        IPetRepository petRepository,
        IDoutorRepository doutorRepository,
        ILogger<MensagemAcompanhamentoService> logger)
    {
        _mensagemRepository = mensagemRepository;
        _petRepository = petRepository;
        _doutorRepository = doutorRepository;
        _logger = logger;
    }

    public async Task<ResultadoPaginado<MensagemResponse>> ListarAsync(MensagemAcompanhamentoFiltro filtro, UsuarioLogado usuario, CancellationToken cancellationToken = default)
    {
        if (usuario.EhTutor)
        {
            filtro.IdTutor = usuario.Id;
        }

        var resultado = await _mensagemRepository.ListarAsync(filtro, cancellationToken);
        return resultado.Mapear(mensagem => mensagem.ParaResposta());
    }

    public async Task<MensagemResponse> BuscarPorIdAsync(string id, UsuarioLogado usuario, CancellationToken cancellationToken = default)
    {
        var mensagem = await ObterMensagemAsync(id, cancellationToken);
        if (usuario.EhTutor && !mensagem.PertenceAoTutor(usuario.Id))
        {
            throw new AcessoNegadoException("Essa mensagem foi enviada para outro tutor.");
        }
        return mensagem.ParaResposta();
    }

    public async Task<MensagemResponse> EnviarAsync(EnviarMensagemRequest request, UsuarioLogado usuario, CancellationToken cancellationToken = default)
    {
        var pet = await _petRepository.BuscarPorIdAsync(request.IdPet, cancellationToken)
            ?? throw new RecursoNaoEncontradoException($"Pet {request.IdPet} não encontrado.");
        var doutor = await _doutorRepository.BuscarPorIdAsync(usuario.Id, cancellationToken)
            ?? throw new RecursoNaoEncontradoException($"Doutor {usuario.Id} não encontrado.");

        var mensagem = MensagemAcompanhamento.Criar(pet, doutor, request.Categoria, request.Assunto, request.Conteudo);
        await _mensagemRepository.AdicionarAsync(mensagem, cancellationToken);

        ClyvoEloTelemetry.MensagensEnviadas.Add(1, new KeyValuePair<string, object?>("categoria", mensagem.Categoria));
        _logger.LogInformation(
            "Mensagem {IdMensagem} enviada pelo doutor {IdDoutor} ao tutor {IdTutor} sobre o pet {IdPet}",
            mensagem.Id, mensagem.IdDoutor, mensagem.IdTutor, mensagem.IdPet);

        return mensagem.ParaResposta();
    }

    public async Task MarcarComoLidaAsync(string id, UsuarioLogado usuario, CancellationToken cancellationToken = default)
    {
        var mensagem = await ObterMensagemAsync(id, cancellationToken);
        if (!mensagem.PertenceAoTutor(usuario.Id))
        {
            throw new AcessoNegadoException("Só o tutor que recebeu a mensagem pode marcá-la como lida.");
        }

        mensagem.MarcarComoLida();
        await _mensagemRepository.AtualizarAsync(mensagem, cancellationToken);
    }

    public async Task RemoverAsync(string id, UsuarioLogado usuario, CancellationToken cancellationToken = default)
    {
        var mensagem = await ObterMensagemAsync(id, cancellationToken);
        if (!mensagem.FoiEnviadaPor(usuario.Id))
        {
            throw new AcessoNegadoException("Só o doutor que enviou a mensagem pode removê-la.");
        }

        await _mensagemRepository.RemoverAsync(id, cancellationToken);
        _logger.LogInformation("Mensagem {IdMensagem} removida pelo doutor {IdDoutor}", id, usuario.Id);
    }

    private async Task<MensagemAcompanhamento> ObterMensagemAsync(string id, CancellationToken cancellationToken)
    {
        return await _mensagemRepository.BuscarPorIdAsync(id, cancellationToken)
            ?? throw new RecursoNaoEncontradoException($"Mensagem {id} não encontrada.");
    }
}
