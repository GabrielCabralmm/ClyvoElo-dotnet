using Microsoft.EntityFrameworkCore;
using Xunit;
using ClyvoElo.Api.Data;
using ClyvoElo.Api.Models;
using ClyvoElo.Api.Services;

namespace ClyvoElo.Tests.Unit;

public class PetServiceTests
{
    private static ClyvoEloContext CriarContextoEmMemoria()
    {
        var options = new DbContextOptionsBuilder<ClyvoEloContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new ClyvoEloContext(options);
    }

    [Fact]
    public async Task AtualizarPesoAsync_PesoInvalido_LancaArgumentException()
    {
        using var context = CriarContextoEmMemoria();
        var tutor = new Tutor { Nome = "Gabriel", Email = "gabriel@teste.com", SenhaHash = "x" };
        var pet = new Pet { Nome = "Toby", Especie = "CACHORRO", Tutor = tutor };
        context.Pets.Add(pet);
        await context.SaveChangesAsync();
        var service = new PetService(context);

        await Assert.ThrowsAsync<ArgumentException>(() => service.AtualizarPesoAsync(pet.IdPet, -5));
    }

    [Fact]
    public async Task AtualizarPesoAsync_PetNaoExiste_RetornaFalse()
    {
        using var context = CriarContextoEmMemoria();
        var service = new PetService(context);

        var resultado = await service.AtualizarPesoAsync(999, 10);

        Assert.False(resultado);
    }

    [Fact]
    public async Task AtualizarPesoAsync_PesoValido_AtualizaEPersiste()
    {
        using var context = CriarContextoEmMemoria();
        var tutor = new Tutor { Nome = "Marina", Email = "marina@teste.com", SenhaHash = "x" };
        var pet = new Pet { Nome = "Mimi", Especie = "GATO", Tutor = tutor, PesoKg = 3 };
        context.Pets.Add(pet);
        await context.SaveChangesAsync();
        var service = new PetService(context);

        var resultado = await service.AtualizarPesoAsync(pet.IdPet, 4.5m);

        Assert.True(resultado);
        var petAtualizado = await context.Pets.FindAsync(pet.IdPet);
        Assert.Equal(4.5m, petAtualizado!.PesoKg);
    }

    [Fact]
    public async Task CriarAsync_DadosValidos_SalvaEDevolvePet()
    {
        using var context = CriarContextoEmMemoria();
        var tutor = new Tutor { Nome = "Felipe", Email = "felipe@teste.com", SenhaHash = "x" };
        context.Tutores.Add(tutor);
        await context.SaveChangesAsync();
        var service = new PetService(context);
        var novoPet = new Pet { IdTutor = tutor.IdTutor, Nome = "Bidu", Especie = "CACHORRO" };

        var criado = await service.CriarAsync(novoPet);

        Assert.True(criado.IdPet > 0);
        Assert.Equal("Bidu", criado.Nome);
    }
}
