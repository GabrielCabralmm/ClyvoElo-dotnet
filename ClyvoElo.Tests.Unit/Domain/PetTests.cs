using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Exceptions;

namespace ClyvoElo.Tests.Unit.Domain;

public class PetTests
{
    [Fact]
    public void Criar_DadosValidos_NormalizaEspecieEmMaiusculo()
    {
        var pet = Pet.Criar(1, " Toby ", "cachorro", "Golden", new DateTime(2020, 1, 1), 30);

        Assert.Equal("Toby", pet.Nome);
        Assert.Equal("CACHORRO", pet.Especie);
        Assert.Equal(30, pet.PesoKg);
        Assert.Equal(1, pet.IdTutor);
    }

    [Theory]
    [InlineData("", "CACHORRO")]
    [InlineData("Toby", " ")]
    public void Criar_NomeOuEspecieVazios_LancaRegraDeNegocioException(string nome, string especie)
    {
        var acao = () => Pet.Criar(1, nome, especie, null, null, null);

        Assert.Throws<RegraDeNegocioException>(acao);
    }

    [Fact]
    public void Criar_DataNascimentoNoFuturo_LancaRegraDeNegocioException()
    {
        var amanha = DateTime.Today.AddDays(1);

        var excecao = Assert.Throws<RegraDeNegocioException>(() => Pet.Criar(1, "Toby", "CACHORRO", null, amanha, null));

        Assert.Contains("futuro", excecao.Message);
    }

    [Fact]
    public void AtualizarPeso_PesoValido_AtualizaPeso()
    {
        var pet = new Pet { Nome = "Mimi", Especie = "GATO", PesoKg = 3 };

        pet.AtualizarPeso(4.5m);

        Assert.Equal(4.5m, pet.PesoKg);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-5)]
    [InlineData(151)]
    public void AtualizarPeso_PesoForaDoIntervalo_LancaRegraDeNegocioException(decimal pesoInvalido)
    {
        var pet = new Pet { Nome = "Mimi", Especie = "GATO", PesoKg = 3 };

        Assert.Throws<RegraDeNegocioException>(() => pet.AtualizarPeso(pesoInvalido));
        Assert.Equal(3, pet.PesoKg);
    }

    [Fact]
    public void PertenceAoTutor_TutorDiferente_RetornaFalse()
    {
        var pet = new Pet { IdTutor = 1 };

        var resultado = pet.PertenceAoTutor(2);

        Assert.False(resultado);
    }
}
