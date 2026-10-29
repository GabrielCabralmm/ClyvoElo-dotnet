using ClyvoElo.Application.Common;
using ClyvoElo.Application.DTOs.Mensagens;
using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Filtros;

namespace ClyvoElo.Application.Services;

public interface IMensagemAcompanhamentoService
{
    Task<ResultadoPaginado<MensagemResponse>> ListarAsync(MensagemAcompanhamentoFiltro filtro, UsuarioLogado usuario, CancellationToken cancellationToken = default);
    Task<MensagemResponse> BuscarPorIdAsync(string id, UsuarioLogado usuario, CancellationToken cancellationToken = default);
    Task<MensagemResponse> EnviarAsync(EnviarMensagemRequest request, UsuarioLogado usuario, CancellationToken cancellationToken = default);
    Task MarcarComoLidaAsync(string id, UsuarioLogado usuario, CancellationToken cancellationToken = default);
    Task RemoverAsync(string id, UsuarioLogado usuario, CancellationToken cancellationToken = default);
}
