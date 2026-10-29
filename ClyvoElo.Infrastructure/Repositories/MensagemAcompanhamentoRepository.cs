using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Filtros;
using ClyvoElo.Domain.Repositories;
using ClyvoElo.Infrastructure.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ClyvoElo.Infrastructure.Repositories;

public class MensagemAcompanhamentoRepository : IMensagemAcompanhamentoRepository
{
    private readonly IMongoCollection<MensagemAcompanhamento> _colecao;

    public MensagemAcompanhamentoRepository(IMongoDatabase database, IOptions<MongoDbSettings> settings)
    {
        _colecao = database.GetCollection<MensagemAcompanhamento>(settings.Value.ColecaoMensagens);
    }

    public async Task<MensagemAcompanhamento?> BuscarPorIdAsync(string id, CancellationToken cancellationToken = default)
    {
        if (!ObjectId.TryParse(id, out _))
        {
            return null;
        }

        return await _colecao.Find(m => m.Id == id).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ResultadoPaginado<MensagemAcompanhamento>> ListarAsync(MensagemAcompanhamentoFiltro filtro, CancellationToken cancellationToken = default)
    {
        var construtor = Builders<MensagemAcompanhamento>.Filter;
        var condicoes = new List<FilterDefinition<MensagemAcompanhamento>>();

        if (filtro.IdPet.HasValue)
        {
            condicoes.Add(construtor.Eq(m => m.IdPet, filtro.IdPet.Value));
        }

        if (filtro.IdTutor.HasValue)
        {
            condicoes.Add(construtor.Eq(m => m.IdTutor, filtro.IdTutor.Value));
        }

        if (filtro.IdDoutor.HasValue)
        {
            condicoes.Add(construtor.Eq(m => m.IdDoutor, filtro.IdDoutor.Value));
        }

        if (!string.IsNullOrWhiteSpace(filtro.Categoria))
        {
            condicoes.Add(construtor.Eq(m => m.Categoria, filtro.Categoria.Trim().ToUpperInvariant()));
        }

        if (filtro.Lida.HasValue)
        {
            condicoes.Add(construtor.Eq(m => m.Lida, filtro.Lida.Value));
        }

        var filtroMongo = condicoes.Count == 0 ? construtor.Empty : construtor.And(condicoes);

        var ordenacao = Builders<MensagemAcompanhamento>.Sort;
        var ordem = filtro.OrdenarPor?.ToLowerInvariant() switch
        {
            "assunto" => filtro.EhDescendente() ? ordenacao.Descending(m => m.Assunto) : ordenacao.Ascending(m => m.Assunto),
            "categoria" => filtro.EhDescendente() ? ordenacao.Descending(m => m.Categoria) : ordenacao.Ascending(m => m.Categoria),
            "data" => filtro.EhDescendente() ? ordenacao.Descending(m => m.DataEnvio) : ordenacao.Ascending(m => m.DataEnvio),
            _ => ordenacao.Descending(m => m.DataEnvio),
        };

        var total = await _colecao.CountDocumentsAsync(filtroMongo, cancellationToken: cancellationToken);
        var itens = await _colecao.Find(filtroMongo)
            .Sort(ordem)
            .Skip(filtro.QuantidadeIgnorada())
            .Limit(filtro.TamanhoPagina)
            .ToListAsync(cancellationToken);

        return new ResultadoPaginado<MensagemAcompanhamento>(itens, filtro.Pagina, filtro.TamanhoPagina, (int)total);
    }

    public Task AdicionarAsync(MensagemAcompanhamento mensagem, CancellationToken cancellationToken = default)
    {
        return _colecao.InsertOneAsync(mensagem, cancellationToken: cancellationToken);
    }

    public Task AtualizarAsync(MensagemAcompanhamento mensagem, CancellationToken cancellationToken = default)
    {
        return _colecao.ReplaceOneAsync(m => m.Id == mensagem.Id, mensagem, cancellationToken: cancellationToken);
    }

    public Task RemoverAsync(string id, CancellationToken cancellationToken = default)
    {
        return _colecao.DeleteOneAsync(m => m.Id == id, cancellationToken);
    }
}
