using ClyvoElo.Domain.Common;
using ClyvoElo.Domain.Filtros;

namespace ClyvoElo.Tests.Unit.Domain;

public class PaginacaoTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(-3, 1)]
    [InlineData(4, 4)]
    public void Pagina_ValorInformado_NuncaFicaMenorQueUm(int informado, int esperado)
    {
        var filtro = new PetFiltro { Pagina = informado };

        Assert.Equal(esperado, filtro.Pagina);
    }

    [Fact]
    public void TamanhoPagina_AcimaDoMaximo_LimitaAoMaximo()
    {
        var filtro = new PetFiltro { TamanhoPagina = 500 };

        Assert.Equal(FiltroPaginado.TamanhoPaginaMaximo, filtro.TamanhoPagina);
    }

    [Fact]
    public void QuantidadeIgnorada_TerceiraPagina_CalculaDeslocamento()
    {
        var filtro = new PetFiltro { Pagina = 3, TamanhoPagina = 10 };

        Assert.Equal(20, filtro.QuantidadeIgnorada());
    }

    [Fact]
    public void EhDescendente_DirecaoDescEmMaiusculo_RetornaTrue()
    {
        var filtro = new PetFiltro { Direcao = "DESC" };

        Assert.True(filtro.EhDescendente());
    }

    [Fact]
    public void TotalPaginas_TotalNaoDivisivel_ArredondaParaCima()
    {
        var resultado = new ResultadoPaginado<int>(new[] { 1, 2, 3 }, 1, 3, 7);

        Assert.Equal(3, resultado.TotalPaginas);
        Assert.True(resultado.TemProximaPagina);
        Assert.False(resultado.TemPaginaAnterior);
    }

    [Fact]
    public void Mapear_ConversorInformado_MantemMetadadosDaPagina()
    {
        var resultado = new ResultadoPaginado<int>(new[] { 1, 2 }, 2, 2, 6);

        var mapeado = resultado.Mapear(n => $"item {n}");

        Assert.Equal(new[] { "item 1", "item 2" }, mapeado.Itens);
        Assert.Equal(2, mapeado.Pagina);
        Assert.Equal(6, mapeado.TotalItens);
    }
}
