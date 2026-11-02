using System.Text.RegularExpressions;
using ClyvoElo.Application.Exceptions;
using ClyvoElo.Domain.Exceptions;

namespace ClyvoElo.Infrastructure.Procedures;

public static partial class ErroOracle
{
    private static readonly HashSet<int> CodigosNaoEncontrado = new() { 20102, 20111, 20211, 20221, 20232, 20243 };

    private static readonly HashSet<int> CodigosFalhaInterna = new() { 20109, 20119, 20219, 20229, 20239, 20249, 20300, 20301, 20302, 20303 };

    public static bool EhErroDosPackages(int numero) => numero is >= 20000 and <= 20999;

    public static Exception? Converter(int numero, string mensagemOracle)
    {
        if (!EhErroDosPackages(numero) || CodigosFalhaInterna.Contains(numero))
        {
            return null;
        }

        var mensagem = ExtrairMensagem(mensagemOracle);

        return CodigosNaoEncontrado.Contains(numero)
            ? new RecursoNaoEncontradoException(mensagem)
            : new RegraDeNegocioException(mensagem);
    }

    public static string ExtrairMensagem(string mensagemOracle)
    {
        var primeiraLinha = mensagemOracle.Split('\n')[0].Trim();
        return PrefixoOra().Replace(primeiraLinha, string.Empty);
    }

    [GeneratedRegex(@"^ORA-\d+:\s*")]
    private static partial Regex PrefixoOra();
}
