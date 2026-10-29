using ClyvoElo.Application.DTOs.LeiturasSensor;
using ClyvoElo.Application.Exceptions;
using ClyvoElo.Application.Mappers;
using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Filtros;
using ClyvoElo.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace ClyvoElo.Application.Services;

public class LeituraSensorService : ILeituraSensorService
{
    private readonly ILeituraSensorRepository _leituraRepository;
    private readonly IDispositivoIotRepository _dispositivoRepository;
    private readonly ILogger<LeituraSensorService> _logger;

    public LeituraSensorService(
        ILeituraSensorRepository leituraRepository,
        IDispositivoIotRepository dispositivoRepository,
        ILogger<LeituraSensorService> logger)
    {
        _leituraRepository = leituraRepository;
        _dispositivoRepository = dispositivoRepository;
        _logger = logger;
    }

    public async Task<ResultadoPaginado<LeituraSensorResponse>> ListarAsync(LeituraSensorFiltro filtro, CancellationToken cancellationToken = default)
    {
        var resultado = await _leituraRepository.ListarAsync(filtro, cancellationToken);
        return resultado.Mapear(leitura => leitura.ParaResposta());
    }

    public async Task<LeituraSensorResponse> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var leitura = await _leituraRepository.BuscarPorIdAsync(id, cancellationToken)
            ?? throw new RecursoNaoEncontradoException($"Leitura {id} não encontrada.");
        return leitura.ParaResposta();
    }

    public async Task<LeituraSensorResponse> RegistrarAsync(RegistrarLeituraRequest request, CancellationToken cancellationToken = default)
    {
        var dispositivo = await _dispositivoRepository.BuscarPorIdAsync(request.IdDispositivo, cancellationToken)
            ?? throw new RecursoNaoEncontradoException($"Dispositivo {request.IdDispositivo} não encontrado.");

        var leitura = LeituraSensor.Criar(dispositivo.IdDispositivo, request.TipoMedida, request.ValorMedido, request.DataHoraLeitura);
        await _leituraRepository.AdicionarAsync(leitura, cancellationToken);

        _logger.LogInformation(
            "Leitura {TipoMedida} = {ValorMedido} registrada para o dispositivo {IdDispositivo}",
            leitura.TipoMedida, leitura.ValorMedido, leitura.IdDispositivo);

        return leitura.ParaResposta();
    }
}
