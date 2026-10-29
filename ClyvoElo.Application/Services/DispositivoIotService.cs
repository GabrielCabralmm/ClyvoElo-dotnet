using ClyvoElo.Application.DTOs.DispositivosIot;
using ClyvoElo.Application.Exceptions;
using ClyvoElo.Application.Mappers;
using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Filtros;
using ClyvoElo.Domain.Repositories;

namespace ClyvoElo.Application.Services;

public class DispositivoIotService : IDispositivoIotService
{
    private readonly IDispositivoIotRepository _dispositivoRepository;

    public DispositivoIotService(IDispositivoIotRepository dispositivoRepository)
    {
        _dispositivoRepository = dispositivoRepository;
    }

    public async Task<ResultadoPaginado<DispositivoIotResponse>> ListarAsync(DispositivoIotFiltro filtro, CancellationToken cancellationToken = default)
    {
        var resultado = await _dispositivoRepository.ListarAsync(filtro, cancellationToken);
        return resultado.Mapear(dispositivo => dispositivo.ParaResposta());
    }

    public async Task<DispositivoIotResponse> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var dispositivo = await _dispositivoRepository.BuscarPorIdAsync(id, cancellationToken)
            ?? throw new RecursoNaoEncontradoException($"Dispositivo {id} não encontrado.");
        return dispositivo.ParaResposta();
    }
}
