using ClyvoElo.Domain.Constantes;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Exceptions;

namespace ClyvoElo.Tests.Unit.Domain;

public class MensagemAcompanhamentoTests
{
    private readonly Pet _pet = new() { IdPet = 5, IdTutor = 1, Nome = "Toby" };
    private readonly Doutor _doutor = new() { IdDoutor = 10, Nome = "Dra. Ana" };

    [Fact]
    public void Criar_DadosValidos_PreencheTutorDoPetENaoLida()
    {
        var mensagem = MensagemAcompanhamento.Criar(_pet, _doutor, "pos_operatorio", "Curativo", "Trocar o curativo hoje.");

        Assert.Equal(1, mensagem.IdTutor);
        Assert.Equal(10, mensagem.IdDoutor);
        Assert.Equal(CategoriasMensagem.PosOperatorio, mensagem.Categoria);
        Assert.False(mensagem.Lida);
        Assert.Null(mensagem.DataLeitura);
    }

    [Fact]
    public void Criar_ConteudoVazio_LancaRegraDeNegocioException()
    {
        Assert.Throws<RegraDeNegocioException>(
            () => MensagemAcompanhamento.Criar(_pet, _doutor, CategoriasMensagem.Orientacao, "Assunto", "   "));
    }

    [Fact]
    public void Criar_ConteudoMuitoLongo_LancaRegraDeNegocioException()
    {
        var conteudo = new string('a', MensagemAcompanhamento.TamanhoMaximoConteudo + 1);

        Assert.Throws<RegraDeNegocioException>(
            () => MensagemAcompanhamento.Criar(_pet, _doutor, CategoriasMensagem.Orientacao, "Assunto", conteudo));
    }

    [Fact]
    public void Criar_CategoriaInvalida_LancaRegraDeNegocioException()
    {
        Assert.Throws<RegraDeNegocioException>(
            () => MensagemAcompanhamento.Criar(_pet, _doutor, "PROMOCAO", "Assunto", "Conteúdo"));
    }

    [Fact]
    public void MarcarComoLida_MensagemNaoLida_DefineDataDeLeitura()
    {
        var mensagem = MensagemAcompanhamento.Criar(_pet, _doutor, CategoriasMensagem.Medicacao, "Remédio", "Dar o remédio às 8h.");

        mensagem.MarcarComoLida();

        Assert.True(mensagem.Lida);
        Assert.NotNull(mensagem.DataLeitura);
    }

    [Fact]
    public void MarcarComoLida_MensagemJaLida_MantemPrimeiraDataDeLeitura()
    {
        var mensagem = MensagemAcompanhamento.Criar(_pet, _doutor, CategoriasMensagem.Medicacao, "Remédio", "Dar o remédio às 8h.");
        mensagem.MarcarComoLida();
        var primeiraLeitura = mensagem.DataLeitura;

        mensagem.MarcarComoLida();

        Assert.Equal(primeiraLeitura, mensagem.DataLeitura);
    }
}
