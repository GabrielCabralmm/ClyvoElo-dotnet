using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Filtros;

namespace ClyvoElo.Domain.Repositories;

public interface IMensagemAcompanhamentoRepository
{
    Task<MensagemAcompanhamento?> BuscarPorIdAsync(string id, CancellationToken cancellationToken = default);
    Task<ResultadoPaginado<MensagemAcompanhamento>> ListarAsync(MensagemAcompanhamentoFiltro filtro, CancellationToken cancellationToken = default);
    Task AdicionarAsync(MensagemAcompanhamento mensagem, CancellationToken cancellationToken = default);
    Task AtualizarAsync(MensagemAcompanhamento mensagem, CancellationToken cancellationToken = default);
    Task RemoverAsync(string id, CancellationToken cancellationToken = default);
}
