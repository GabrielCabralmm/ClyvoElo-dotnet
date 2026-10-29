using ClyvoElo.Application.DTOs.DispositivosIot;
using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Filtros;

namespace ClyvoElo.Application.Services;

public interface IDispositivoIotService
{
    Task<ResultadoPaginado<DispositivoIotResponse>> ListarAsync(DispositivoIotFiltro filtro, CancellationToken cancellationToken = default);
    Task<DispositivoIotResponse> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default);
}
