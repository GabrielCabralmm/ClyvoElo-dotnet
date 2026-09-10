using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace ClyvoElo.Tests.Integration;

[CollectionDefinition("ClyvoEloApi")]
public class ClyvoEloApiCollection : ICollectionFixture<WebApplicationFactory<Program>>
{
}
