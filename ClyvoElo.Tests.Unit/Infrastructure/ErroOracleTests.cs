using ClyvoElo.Application.Exceptions;
using ClyvoElo.Domain.Exceptions;
using ClyvoElo.Infrastructure.Procedures;

namespace ClyvoElo.Tests.Unit.Infrastructure;

public class ErroOracleTests
{
    private const string MensagemComPilha =
        "ORA-20211: Alerta: pet 999 nao encontrado\nORA-06512: at \"RM563230.PKG_CLYVO_ALERTA\", line 120\nORA-06512: at line 1";

    [Fact]
    public void ExtrairMensagem_RemoveOCodigoEAPilhaDoOracle()
    {
        Assert.Equal("Alerta: pet 999 nao encontrado", ErroOracle.ExtrairMensagem(MensagemComPilha));
    }

    [Theory]
    [InlineData(20102)]
    [InlineData(20211)]
    [InlineData(20221)]
    [InlineData(20232)]
    public void Converter_CodigoDeRegistroInexistente_ViraRecursoNaoEncontrado(int numero)
    {
        Assert.IsType<RecursoNaoEncontradoException>(ErroOracle.Converter(numero, MensagemComPilha));
    }

    [Theory]
    [InlineData(20203)]
    [InlineData(20212)]
    [InlineData(20222)]
    [InlineData(20241)]
    public void Converter_CodigoDeValidacao_ViraRegraDeNegocio(int numero)
    {
        var excecao = ErroOracle.Converter(numero, "ORA-20203: Pontuacao: quantidade de fatores nao pode ser zero");

        Assert.IsType<RegraDeNegocioException>(excecao);
        Assert.Equal("Pontuacao: quantidade de fatores nao pode ser zero", excecao!.Message);
    }

    [Theory]
    [InlineData(20219)]
    [InlineData(20300)]
    [InlineData(1400)]
    public void Converter_FalhaInternaOuErroQueNaoEDosPackages_NaoConverte(int numero)
    {
        Assert.Null(ErroOracle.Converter(numero, MensagemComPilha));
    }
}
