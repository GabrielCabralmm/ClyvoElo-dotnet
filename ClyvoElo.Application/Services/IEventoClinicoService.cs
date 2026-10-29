using ClyvoElo.Application.Common;
using ClyvoElo.Application.DTOs.EventosClinicos;
using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Filtros;

namespace ClyvoElo.Application.Services;

public interface IEventoClinicoService
{
    Task<ResultadoPaginado<EventoClinicoResponse>> ListarAsync(EventoClinicoFiltro filtro, CancellationToken cancellationToken = default);
    Task<EventoClinicoResponse> BuscarPorIdAsync(int id, CancellationToken cancellationToken = default);
    Task<EventoClinicoResponse> CriarAsync(CriarEventoClinicoRequest request, UsuarioLogado usuario, CancellationToken cancellationToken = default);
    Task AtualizarStatusAsync(int id, string novoStatus, CancellationToken cancellationToken = default);
    Task RemoverAsync(int id, CancellationToken cancellationToken = default);
}
