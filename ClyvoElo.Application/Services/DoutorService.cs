using ClyvoElo.Application.DTOs.Doutores;
using ClyvoElo.Application.Exceptions;
using ClyvoElo.Application.Mappers;
using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Filtros;
using ClyvoElo.Domain.Repositories;

namespace ClyvoElo.Application.Services;

public class DoutorService : IDoutorService
{
    private readonly IDoutorRepository _doutorRepository;

    public DoutorService(IDoutorRepository doutorRepository)
    {
        _doutorRepository = doutorRepository;
    }

    public async Task<ResultadoPaginado<DoutorResponse>> ListarAsync(DoutorFiltro filtro, CancellationToken cancellationToken = default)
    {
        var resultado = await _doutorRepository.ListarAsync(filtro, cancellationToken);
        return resultado.Mapear(doutor => doutor.ParaResposta());
    }

    public async Task<DoutorResponse> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var doutor = await _doutorRepository.BuscarPorIdAsync(id, cancellationToken)
            ?? throw new RecursoNaoEncontradoException($"Doutor {id} não encontrado.");
        return doutor.ParaResposta();
    }
}
