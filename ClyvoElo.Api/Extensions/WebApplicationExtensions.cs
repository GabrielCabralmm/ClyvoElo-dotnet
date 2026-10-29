using ClyvoElo.Api.HealthChecks;
using ClyvoElo.Application.Interfaces;
using ClyvoElo.Infrastructure.Persistence;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;

namespace ClyvoElo.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder AdicionarLogEstruturado(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((context, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .MinimumLevel.Information()
                .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Aplicacao", "ClyvoElo.Api")
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] ({CorrelationId}) {SourceContext}: {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    "logs/clyvoelo-.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({CorrelationId}) {SourceContext}: {Message:lj}{NewLine}{Exception}");
        });

        return builder;
    }

    public static WebApplication MapearHealthChecks(this WebApplication app)
    {
        app.MapHealthChecks("/health", new HealthCheckOptions
        {
            ResponseWriter = HealthCheckResponseWriter.WriteResponse,
        });

        app.MapHealthChecks("/health/live", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(HealthCheckTags.Live),
            ResponseWriter = HealthCheckResponseWriter.WriteResponse,
        });

        app.MapHealthChecks("/health/ready", new HealthCheckOptions
        {
            Predicate = check => check.Tags.Contains(HealthCheckTags.Ready),
            ResponseWriter = HealthCheckResponseWriter.WriteResponse,
        });

        return app;
    }

    public static async Task AplicarMigracoesAsync(this WebApplication app)
    {
        if (!app.Configuration.GetValue<bool>("Database:AplicarMigracoesAoIniciar"))
        {
            return;
        }

        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ClyvoEloContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        app.Logger.LogInformation("Aplicando migrações do EF Core no Oracle");
        await context.Database.MigrateAsync();
        await DadosIniciais.SemearAsync(context, passwordHasher);
    }
}
