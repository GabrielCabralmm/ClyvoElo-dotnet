namespace ClyvoElo.Tests.Integration.Fixtures;

[CollectionDefinition(Nome)]
public class ClyvoEloApiCollection : ICollectionFixture<ClyvoEloApiFactory>
{
    public const string Nome = "ClyvoEloApi";
}
