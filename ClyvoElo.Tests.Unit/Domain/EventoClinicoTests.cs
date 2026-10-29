using ClyvoElo.Domain.Constantes;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Exceptions;

namespace ClyvoElo.Tests.Unit.Domain;

public class EventoClinicoTests
{
    [Fact]
    public void Criar_TipoEmMinusculo_NormalizaParaMaiusculo()
    {
        var evento = EventoClinico.Criar(1, 2, "vacina", "V10", DateTime.Today, "concluido");

        Assert.Equal(TiposEventoClinico.Vacina, evento.TipoEvento);
        Assert.Equal(StatusEventoClinico.Concluido, evento.Status);
    }

    [Fact]
    public void Criar_SemStatus_UsaPendente()
    {
        var evento = EventoClinico.Criar(1, 2, TiposEventoClinico.Exame, null, DateTime.Today, null);

        Assert.Equal(StatusEventoClinico.Pendente, evento.Status);
    }

    [Fact]
    public void Criar_TipoInvalido_LancaRegraDeNegocioException()
    {
        var excecao = Assert.Throws<RegraDeNegocioException>(
            () => EventoClinico.Criar(1, 2, "CIRURGIA_ESTETICA", null, DateTime.Today, null));

        Assert.Contains("VACINA", excecao.Message);
    }

    [Fact]
    public void AtualizarStatus_StatusValido_AlteraStatus()
    {
        var evento = EventoClinico.Criar(1, 2, TiposEventoClinico.Checkup, null, DateTime.Today, null);

        evento.AtualizarStatus("atrasado");

        Assert.Equal(StatusEventoClinico.Atrasado, evento.Status);
    }

    [Fact]
    public void AtualizarStatus_StatusInvalido_LancaRegraDeNegocioExceptionEMantemStatus()
    {
        var evento = EventoClinico.Criar(1, 2, TiposEventoClinico.Checkup, null, DateTime.Today, null);

        Assert.Throws<RegraDeNegocioException>(() => evento.AtualizarStatus("CANCELADO"));
        Assert.Equal(StatusEventoClinico.Pendente, evento.Status);
    }
}
