using ClyvoElo.Application.DTOs.LeiturasSensor;
using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Filtros;

namespace ClyvoElo.Application.Services;

public interface ILeituraSensorService
{
    Task<ResultadoPaginado<LeituraSensorResponse>> ListarAsync(LeituraSensorFiltro filtro, CancellationToken cancellationToken = default);
    Task<LeituraSensorResponse> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<LeituraSensorResponse> RegistrarAsync(RegistrarLeituraRequest request, CancellationToken cancellationToken = default);
}
