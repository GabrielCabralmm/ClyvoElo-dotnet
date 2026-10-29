using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Exceptions;

namespace ClyvoElo.Tests.Unit.Domain;

public class LeituraSensorTests
{
    [Fact]
    public void Criar_SemDataHora_UsaMomentoAtual()
    {
        var antes = DateTime.Now.AddSeconds(-1);

        var leitura = LeituraSensor.Criar(1, "AGUA_ML", 500, null);

        Assert.True(leitura.DataHoraLeitura >= antes);
        Assert.Equal(500, leitura.ValorMedido);
    }

    [Fact]
    public void Criar_TipoMedidaVazio_LancaRegraDeNegocioException()
    {
        Assert.Throws<RegraDeNegocioException>(() => LeituraSensor.Criar(1, "", 10, null));
    }

    [Fact]
    public void Criar_DataNoFuturo_LancaRegraDeNegocioException()
    {
        Assert.Throws<RegraDeNegocioException>(() => LeituraSensor.Criar(1, "AGUA_ML", 10, DateTime.Now.AddDays(1)));
    }
}
