using ClyvoElo.Application.Common;
using ClyvoElo.Domain.Constantes;
using ClyvoElo.Domain.Entities;

namespace ClyvoElo.Tests.Unit.Fixtures;

public class DadosDeTesteFixture
{
    public UsuarioLogado TutorLogado { get; } = new(1, Papeis.Tutor);
    public UsuarioLogado OutroTutorLogado { get; } = new(2, Papeis.Tutor);
    public UsuarioLogado DoutorLogado { get; } = new(10, Papeis.Doutor);

    public Tutor CriarTutor(int id = 1) => new()
    {
        IdTutor = id,
        Nome = "Gabriel",
        Email = "gabriel@email.com",
        SenhaHash = "hash-da-senha",
    };

    public Doutor CriarDoutor(int id = 10) => new()
    {
        IdDoutor = id,
        IdClinica = 1,
        Nome = "Dra. Ana Ferreira",
        Crmv = "CRMV-SP 12345",
        Email = "ana.ferreira@clyvovet.com",
        SenhaHash = "hash-da-senha",
    };

    public Pet CriarPet(int id = 5, int idTutor = 1) => new()
    {
        IdPet = id,
        IdTutor = idTutor,
        Nome = "Toby",
        Especie = "CACHORRO",
        PesoKg = 30,
    };

    public MensagemAcompanhamento CriarMensagem(string id = "6650f1c2a1b2c3d4e5f60718", int idTutor = 1, int idDoutor = 10) => new()
    {
        Id = id,
        IdPet = 5,
        NomePet = "Toby",
        IdTutor = idTutor,
        IdDoutor = idDoutor,
        NomeDoutor = "Dra. Ana Ferreira",
        Categoria = CategoriasMensagem.PosOperatorio,
        Assunto = "Curativo",
        Conteudo = "Trocar o curativo a cada 12 horas.",
        DataEnvio = DateTime.UtcNow,
    };
}

[CollectionDefinition("DadosDeTeste")]
public class DadosDeTesteCollection : ICollectionFixture<DadosDeTesteFixture>
{
}
