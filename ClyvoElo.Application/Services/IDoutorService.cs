using ClyvoElo.Application.DTOs.Doutores;
using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Filtros;

namespace ClyvoElo.Application.Services;

public interface IDoutorService
{
    Task<ResultadoPaginado<DoutorResponse>> ListarAsync(DoutorFiltro filtro, CancellationToken cancellationToken = default);
    Task<DoutorResponse> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default);
}
