using ClyvoElo.Application.Interfaces;
using ClyvoElo.Domain.Repositories;
using ClyvoElo.Infrastructure.Persistence;
using ClyvoElo.Tests.Integration.Fakes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

namespace ClyvoElo.Tests.Integration.Fixtures;

public class ClyvoEloApiFactory : WebApplicationFactory<Program>
{
    private readonly string _nomeBanco = $"ClyvoEloTestes-{Guid.NewGuid()}";

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var registrosDoContexto = services
                .Where(s => s.ServiceType == typeof(DbContextOptions<ClyvoEloContext>)
                         || s.ServiceType == typeof(DbContextOptions)
                         || (s.ServiceType.IsGenericType && s.ServiceType.GetGenericTypeDefinition().Name.StartsWith("IDbContextOptionsConfiguration")))
                .ToList();
            foreach (var registro in registrosDoContexto)
            {
                services.Remove(registro);
            }

            services.AddDbContext<ClyvoEloContext>(options => options.UseInMemoryDatabase(_nomeBanco));

            services.RemoveAll<IMensagemAcompanhamentoRepository>();
            services.AddSingleton<IMensagemAcompanhamentoRepository, MensagemAcompanhamentoRepositoryEmMemoria>();

            services.Configure<HealthCheckServiceOptions>(options =>
            {
                foreach (var registro in options.Registrations.Where(r => r.Name is "mongodb" or "servico_externo"))
                {
                    registro.Factory = _ => new HealthCheckSempreSaudavel();
                }
            });
        });
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        using var scope = host.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ClyvoEloContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        DadosIniciais.SemearAsync(context, passwordHasher).GetAwaiter().GetResult();

        return host;
    }
}
