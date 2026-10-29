using ClyvoElo.Application.Interfaces;
using ClyvoElo.Domain.Repositories;
using ClyvoElo.Infrastructure.HealthChecks;
using ClyvoElo.Infrastructure.Mongo;
using ClyvoElo.Infrastructure.Persistence;
using ClyvoElo.Infrastructure.Repositories;
using ClyvoElo.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ClyvoElo.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ClyvoEloContext>(options =>
            options.UseOracle(configuration.GetConnectionString("OracleDb")));

        services.AddScoped<ITutorRepository, TutorRepository>();
        services.AddScoped<IDoutorRepository, DoutorRepository>();
        services.AddScoped<IPetRepository, PetRepository>();
        services.AddScoped<IEventoClinicoRepository, EventoClinicoRepository>();
        services.AddScoped<IDispositivoIotRepository, DispositivoIotRepository>();
        services.AddScoped<ILeituraSensorRepository, LeituraSensorRepository>();

        services.AddMongoDb(configuration);

        services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.Secao));
        services.AddSingleton<ITokenService, JwtTokenService>();
        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

        services.AddHttpClient(ServicoExternoHealthCheck.NomeCliente, client => client.Timeout = TimeSpan.FromSeconds(5));

        return services;
    }

    private static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration configuration)
    {
        MongoDbMapeamentos.Registrar();

        services.Configure<MongoDbSettings>(configuration.GetSection(MongoDbSettings.Secao));

        services.AddSingleton<IMongoClient>(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            var clientSettings = MongoClientSettings.FromConnectionString(settings.ConnectionString);
            clientSettings.ServerSelectionTimeout = TimeSpan.FromSeconds(5);
            clientSettings.ConnectTimeout = TimeSpan.FromSeconds(5);
            return new MongoClient(clientSettings);
        });

        services.AddSingleton(provider =>
        {
            var settings = provider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
            return provider.GetRequiredService<IMongoClient>().GetDatabase(settings.DatabaseName);
        });

        services.AddScoped<IMensagemAcompanhamentoRepository, MensagemAcompanhamentoRepository>();

        return services;
    }
}
