using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Filtros;
using ClyvoElo.Domain.Repositories;

namespace ClyvoElo.Tests.Integration.Fakes;

public class MensagemAcompanhamentoRepositoryEmMemoria : IMensagemAcompanhamentoRepository
{
    private readonly List<MensagemAcompanhamento> _mensagens = new();
    private readonly object _trava = new();

    public Task<MensagemAcompanhamento?> BuscarPorIdAsync(string id, CancellationToken cancellationToken = default)
    {
        lock (_trava)
        {
            return Task.FromResult(_mensagens.FirstOrDefault(m => m.Id == id));
        }
    }

    public Task<ResultadoPaginado<MensagemAcompanhamento>> ListarAsync(MensagemAcompanhamentoFiltro filtro, CancellationToken cancellationToken = default)
    {
        lock (_trava)
        {
            var consulta = _mensagens.AsEnumerable();
            if (filtro.IdPet.HasValue) consulta = consulta.Where(m => m.IdPet == filtro.IdPet.Value);
            if (filtro.IdTutor.HasValue) consulta = consulta.Where(m => m.IdTutor == filtro.IdTutor.Value);
            if (filtro.IdDoutor.HasValue) consulta = consulta.Where(m => m.IdDoutor == filtro.IdDoutor.Value);
            if (filtro.Lida.HasValue) consulta = consulta.Where(m => m.Lida == filtro.Lida.Value);
            if (!string.IsNullOrWhiteSpace(filtro.Categoria)) consulta = consulta.Where(m => m.Categoria == filtro.Categoria.ToUpperInvariant());

            var todos = consulta.OrderByDescending(m => m.DataEnvio).ToList();
            var pagina = todos.Skip(filtro.QuantidadeIgnorada()).Take(filtro.TamanhoPagina).ToList();
            return Task.FromResult(new ResultadoPaginado<MensagemAcompanhamento>(pagina, filtro.Pagina, filtro.TamanhoPagina, todos.Count));
        }
    }

    public Task AdicionarAsync(MensagemAcompanhamento mensagem, CancellationToken cancellationToken = default)
    {
        lock (_trava)
        {
            mensagem.Id = Guid.NewGuid().ToString("N")[..24];
            _mensagens.Add(mensagem);
        }
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(MensagemAcompanhamento mensagem, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task RemoverAsync(string id, CancellationToken cancellationToken = default)
    {
        lock (_trava)
        {
            _mensagens.RemoveAll(m => m.Id == id);
        }
        return Task.CompletedTask;
    }
}
