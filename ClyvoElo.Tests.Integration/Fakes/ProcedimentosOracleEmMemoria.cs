using ClyvoElo.Application.Exceptions;
using ClyvoElo.Application.Interfaces;
using ClyvoElo.Domain.Exceptions;

namespace ClyvoElo.Tests.Integration.Fakes;

public class ProcedimentosOracleEmMemoria : IProcedimentosOracle
{
    private readonly HashSet<int> _alertas = new() { 1, 2, 3 };
    private int _proximoId = 4;

    public Task<int> RegistrarAlertaAsync(int idPet, string origem, string prioridade, decimal somaFatores, decimal qtdFatores, string mensagem, CancellationToken cancellationToken = default)
    {
        if (idPet == 999)
        {
            throw new RecursoNaoEncontradoException("Alerta: pet 999 nao encontrado");
        }

        if (qtdFatores == 0)
        {
            throw new RegraDeNegocioException("Pontuacao: quantidade de fatores nao pode ser zero");
        }

        lock (_alertas)
        {
            var id = _proximoId++;
            _alertas.Add(id);
            return Task.FromResult(id);
        }
    }

    public Task AtualizarStatusAlertaAsync(int idAlerta, string status, CancellationToken cancellationToken = default)
    {
        GarantirQueExiste(idAlerta);
        return Task.CompletedTask;
    }

    public Task ExcluirAlertaAsync(int idAlerta, CancellationToken cancellationToken = default)
    {
        GarantirQueExiste(idAlerta);
        lock (_alertas)
        {
            _alertas.Remove(idAlerta);
        }
        return Task.CompletedTask;
    }

    public Task<string> GerarRelatorioAlertasAsync(string? origem, CancellationToken cancellationToken = default)
    {
        return Task.FromResult("Origem      Prioridade   Pontuacao\nCLINICO     ALTA             15.20\nSub Total                    15.20\nTotal Geral                  15.20\n");
    }

    public Task<string> ListarPetsJsonAsync(string? especie, CancellationToken cancellationToken = default)
    {
        return Task.FromResult("[\n  {\"id_pet\":1,\"nome_pet\":\"Toby\",\"especie\":\"CACHORRO\",\"tutor\":{\"nome\":\"Gabriel Souza\",\"email\":\"gabriel@email.com\"}}\n]");
    }

    public Task<string> ExportarDatasetAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult("[\n{\"_id\":1,\"nome\":\"Toby\",\"especie\":\"CACHORRO\",\"tutor\":{\"id_tutor\":1,\"nome\":\"Gabriel Souza\",\"email\":\"gabriel@email.com\"},\"eventos_clinicos\":[],\"dispositivos\":[],\"alertas\":[]}\n]");
    }

    private void GarantirQueExiste(int idAlerta)
    {
        lock (_alertas)
        {
            if (!_alertas.Contains(idAlerta))
            {
                throw new RecursoNaoEncontradoException($"Alerta: id {idAlerta} nao encontrado");
            }
        }
    }
}
