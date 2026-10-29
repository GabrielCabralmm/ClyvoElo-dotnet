using Microsoft.Extensions.Diagnostics.HealthChecks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace ClyvoElo.Infrastructure.HealthChecks;

public class MongoDbHealthCheck : IHealthCheck
{
    private readonly IMongoDatabase _database;

    public MongoDbHealthCheck(IMongoDatabase database)
    {
        _database = database;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
    {
        try
        {
            await _database.RunCommandAsync((Command<BsonDocument>)"{ ping: 1 }", cancellationToken: cancellationToken);
            return HealthCheckResult.Healthy("Conexão com o MongoDB está OK.");
        }
        catch (Exception ex)
        {
            return HealthCheckResult.Unhealthy("Não foi possível conectar ao MongoDB.", ex);
        }
    }
}
