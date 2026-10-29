using ClyvoElo.Application.Interfaces;
using ClyvoElo.Domain.Constantes;
using ClyvoElo.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ClyvoElo.Infrastructure.Persistence;

public static class DadosIniciais
{
    public const string SenhaPadrao = "clyvo123";

    public static async Task SemearAsync(ClyvoEloContext context, IPasswordHasher passwordHasher, CancellationToken cancellationToken = default)
    {
        if (await context.Tutores.AnyAsync(cancellationToken))
        {
            return;
        }

        var senhaHash = passwordHasher.GerarHash(SenhaPadrao);

        var clinica = new Clinica { Nome = "Clyvo Vet Paulista", Telefone = "11 3000-1000" };
        var doutora = new Doutor
        {
            Clinica = clinica,
            Nome = "Dra. Ana Ferreira",
            Crmv = "CRMV-SP 12345",
            Email = "ana.ferreira@clyvovet.com",
            SenhaHash = senhaHash,
        };
        var doutor = new Doutor
        {
            Clinica = clinica,
            Nome = "Dr. Ricardo Lima",
            Crmv = "CRMV-SP 67890",
            Email = "ricardo.lima@clyvovet.com",
            SenhaHash = senhaHash,
        };

        var gabriel = new Tutor
        {
            Nome = "Gabriel Mariano",
            Email = "gabriel@email.com",
            SenhaHash = senhaHash,
            Telefone = "11 99999-0001",
            DataCadastro = DateTime.Today.AddMonths(-6),
        };
        var marina = new Tutor
        {
            Nome = "Marina Souza",
            Email = "marina@email.com",
            SenhaHash = senhaHash,
            Telefone = "11 99999-0002",
            DataCadastro = DateTime.Today.AddMonths(-3),
        };

        var toby = new Pet { Tutor = gabriel, Nome = "Toby", Especie = "CACHORRO", Raca = "Golden Retriever", DataNascimento = new DateTime(2020, 3, 14), PesoKg = 31.5m };
        var mimi = new Pet { Tutor = gabriel, Nome = "Mimi", Especie = "GATO", Raca = "Siamês", DataNascimento = new DateTime(2022, 8, 2), PesoKg = 4.2m };
        var thor = new Pet { Tutor = marina, Nome = "Thor", Especie = "CACHORRO", Raca = "Bulldog Francês", DataNascimento = new DateTime(2021, 11, 20), PesoKg = 12.8m };

        var comedouro = new DispositivoIot { Pet = toby, TipoDispositivo = "POTE_RACAO", DataInstalacao = DateTime.Today.AddDays(-30) };
        var bebedouro = new DispositivoIot { Pet = toby, TipoDispositivo = "POTE_AGUA", DataInstalacao = DateTime.Today.AddDays(-30) };

        context.AddRange(clinica, doutora, doutor, gabriel, marina, toby, mimi, thor, comedouro, bebedouro);

        context.EventosClinicos.AddRange(
            new EventoClinico { Pet = toby, Doutor = doutora, TipoEvento = TiposEventoClinico.Vacina, Descricao = "Vacina V10 anual", DataEvento = DateTime.Today.AddDays(-20), Status = StatusEventoClinico.Concluido },
            new EventoClinico { Pet = toby, Doutor = doutora, TipoEvento = TiposEventoClinico.PosOperatorio, Descricao = "Retorno da castração", DataEvento = DateTime.Today.AddDays(5), Status = StatusEventoClinico.Pendente },
            new EventoClinico { Pet = mimi, Doutor = doutor, TipoEvento = TiposEventoClinico.Checkup, Descricao = "Check-up semestral", DataEvento = DateTime.Today.AddDays(-2), Status = StatusEventoClinico.Atrasado },
            new EventoClinico { Pet = thor, Doutor = doutor, TipoEvento = TiposEventoClinico.Exame, Descricao = "Hemograma completo", DataEvento = DateTime.Today.AddDays(10), Status = StatusEventoClinico.Pendente });

        context.LeiturasSensor.AddRange(
            new LeituraSensor { Dispositivo = comedouro, TipoMedida = "RACAO_GRAMAS", ValorMedido = 320, DataHoraLeitura = DateTime.Now.AddHours(-6) },
            new LeituraSensor { Dispositivo = comedouro, TipoMedida = "RACAO_GRAMAS", ValorMedido = 180, DataHoraLeitura = DateTime.Now.AddHours(-1) },
            new LeituraSensor { Dispositivo = bebedouro, TipoMedida = "AGUA_ML", ValorMedido = 850, DataHoraLeitura = DateTime.Now.AddHours(-2) });

        await context.SaveChangesAsync(cancellationToken);
    }
}
