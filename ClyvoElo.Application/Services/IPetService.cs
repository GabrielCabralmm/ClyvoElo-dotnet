using ClyvoElo.Application.Common;
using ClyvoElo.Application.DTOs.Pets;
using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Filtros;

namespace ClyvoElo.Application.Services;

public interface IPetService
{
    Task<ResultadoPaginado<PetResponse>> ListarAsync(PetFiltro filtro, CancellationToken cancellationToken = default);
    Task<ResultadoPaginado<PetResponse>> ListarDoTutorAsync(int idTutor, PetFiltro filtro, CancellationToken cancellationToken = default);
    Task<PetResponse> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<PetResponse> CriarAsync(CriarPetRequest request, UsuarioLogado usuario, CancellationToken cancellationToken = default);
    Task AtualizarPesoAsync(int id, decimal novoPeso, UsuarioLogado usuario, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, UsuarioLogado usuario, CancellationToken cancellationToken = default);
}
