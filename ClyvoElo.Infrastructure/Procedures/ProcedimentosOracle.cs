using System.Data;
using ClyvoElo.Application.Interfaces;
using ClyvoElo.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace ClyvoElo.Infrastructure.Procedures;

public class ProcedimentosOracle : IProcedimentosOracle
{
    private readonly ClyvoEloContext _context;

    public ProcedimentosOracle(ClyvoEloContext context)
    {
        _context = context;
    }

    public Task<int> RegistrarAlertaAsync(int idPet, string origem, string prioridade, decimal somaFatores, decimal qtdFatores, string mensagem, CancellationToken cancellationToken = default)
    {
        var idAlerta = new OracleParameter("p_id_alerta", OracleDbType.Int32, ParameterDirection.Output);

        return ExecutarAsync("pkg_clyvo_alerta.prc_registrar_alerta",
            new[]
            {
                Entrada("p_id_pet", OracleDbType.Int32, idPet),
                Entrada("p_origem", OracleDbType.Varchar2, origem),
                Entrada("p_prioridade", OracleDbType.Varchar2, prioridade),
                Entrada("p_soma_fatores", OracleDbType.Decimal, somaFatores),
                Entrada("p_qtd_fatores", OracleDbType.Decimal, qtdFatores),
                Entrada("p_mensagem", OracleDbType.Varchar2, mensagem),
                idAlerta,
            },
            () => ((OracleDecimal)idAlerta.Value).ToInt32(),
            cancellationToken);
    }

    public Task AtualizarStatusAlertaAsync(int idAlerta, string status, CancellationToken cancellationToken = default)
    {
        return ExecutarAsync("pkg_clyvo_alerta.prc_atualizar_status",
            new[]
            {
                Entrada("p_id_alerta", OracleDbType.Int32, idAlerta),
                Entrada("p_status", OracleDbType.Varchar2, status),
            },
            () => true,
            cancellationToken);
    }

    public Task ExcluirAlertaAsync(int idAlerta, CancellationToken cancellationToken = default)
    {
        return ExecutarAsync("pkg_clyvo_alerta.prc_excluir_alerta",
            new[] { Entrada("p_id_alerta", OracleDbType.Int32, idAlerta) },
            () => true,
            cancellationToken);
    }

    public Task<string> GerarRelatorioAlertasAsync(string? origem, CancellationToken cancellationToken = default)
    {
        var relatorio = new OracleParameter("p_relatorio", OracleDbType.Clob, ParameterDirection.Output);

        return ExecutarAsync("pkg_clyvo_alerta.prc_relatorio_alertas",
            new[] { Entrada("p_origem", OracleDbType.Varchar2, origem), relatorio },
            () => LerClob(relatorio),
            cancellationToken);
    }

    public Task<string> ListarPetsJsonAsync(string? especie, CancellationToken cancellationToken = default)
    {
        var json = new OracleParameter("p_json", OracleDbType.Clob, ParameterDirection.Output);

        return ExecutarAsync("pkg_clyvo_pet.prc_lista_pets_json",
            new[] { Entrada("p_especie", OracleDbType.Varchar2, especie), json },
            () => LerClob(json),
            cancellationToken);
    }

    public Task<string> ExportarDatasetAsync(CancellationToken cancellationToken = default)
    {
        var json = new OracleParameter("p_json", OracleDbType.Clob, ParameterDirection.Output);

        return ExecutarAsync("pkg_clyvo_pet.prc_exportar_dataset",
            new[] { json },
            () => LerClob(json),
            cancellationToken);
    }

    private async Task<T> ExecutarAsync<T>(string procedure, OracleParameter[] parametros, Func<T> lerResultado, CancellationToken cancellationToken)
    {
        await _context.Database.OpenConnectionAsync(cancellationToken);
        try
        {
            using var comando = (OracleCommand)_context.Database.GetDbConnection().CreateCommand();
            comando.CommandText = procedure;
            comando.CommandType = CommandType.StoredProcedure;
            comando.BindByName = true;
            foreach (var parametro in parametros)
            {
                comando.Parameters.Add(parametro);
            }

            await comando.ExecuteNonQueryAsync(cancellationToken);
            return lerResultado();
        }
        catch (OracleException ex) when (ErroOracle.Converter(ex.Number, ex.Message) is { } convertida)
        {
            throw convertida;
        }
        finally
        {
            await _context.Database.CloseConnectionAsync();
        }
    }

    private static OracleParameter Entrada(string nome, OracleDbType tipo, object? valor)
    {
        return new OracleParameter(nome, tipo, valor ?? DBNull.Value, ParameterDirection.Input);
    }

    private static string LerClob(OracleParameter parametro)
    {
        if (parametro.Value is OracleClob clob)
        {
            using (clob)
            {
                return clob.IsNull ? string.Empty : clob.Value;
            }
        }

        return string.Empty;
    }
}
