using System.Text;
using ClyvoElo.Api.HealthChecks;
using ClyvoElo.Api.Middlewares;
using ClyvoElo.Application.Observabilidade;
using ClyvoElo.Infrastructure.HealthChecks;
using ClyvoElo.Infrastructure.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace ClyvoElo.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AdicionarAutenticacaoJwt(this IServiceCollection services, IConfiguration configuration)
    {
        var jwt = configuration.GetSection(JwtSettings.Secao).Get<JwtSettings>() ?? new JwtSettings();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwt.Issuer,
                    ValidAudience = jwt.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                };
            });

        services.AddAuthorization();
        return services;
    }

    public static IServiceCollection AdicionarDocumentacaoSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "CLYVO Elo API",
                Version = "v1",
                Description = "API do CLYVO Elo (Challenge FIAP 2026 - CLYVO VET): continuidade do cuidado do pet entre clínica e tutor, " +
                              "com eventos clínicos, dispositivos IoT, leituras de sensores e mensagens de acompanhamento.",
            });

            options.EnableAnnotations();

            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Cole aqui o token devolvido pelo login (sem escrever \"Bearer\" na frente)",
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" },
                    },
                    Array.Empty<string>()
                },
            });
        });

        return services;
    }

    public static IServiceCollection AdicionarObservabilidade(this IServiceCollection services, IHostEnvironment environment)
    {
        var exportarNoConsole = !environment.IsEnvironment("Testing");

        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(ClyvoEloTelemetry.NomeServico))
            .WithTracing(tracing =>
            {
                tracing
                    .AddSource(ClyvoEloTelemetry.NomeServico)
                    .AddAspNetCoreInstrumentation(options => options.Filter = context => !context.Request.Path.StartsWithSegments("/health"))
                    .AddHttpClientInstrumentation();

                if (exportarNoConsole)
                {
                    tracing.AddConsoleExporter();
                }
            })
            .WithMetrics(metrics =>
            {
                metrics
                    .AddMeter(ClyvoEloTelemetry.NomeServico)
                    .AddAspNetCoreInstrumentation()
                    .AddRuntimeInstrumentation();

                if (exportarNoConsole)
                {
                    metrics.AddConsoleExporter();
                }
            });

        return services;
    }

    public static IServiceCollection AdicionarHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks()
            .AddCheck<ApiSelfHealthCheck>("api", tags: new[] { HealthCheckTags.Live })
            .AddCheck<OracleHealthCheck>("oracle_db", tags: new[] { HealthCheckTags.Ready, HealthCheckTags.Banco }, timeout: TimeSpan.FromSeconds(10))
            .AddCheck<MongoDbHealthCheck>("mongodb", tags: new[] { HealthCheckTags.Ready, HealthCheckTags.Banco }, timeout: TimeSpan.FromSeconds(10))
            .AddCheck<ServicoExternoHealthCheck>("servico_externo", tags: new[] { HealthCheckTags.Ready, HealthCheckTags.Externo }, timeout: TimeSpan.FromSeconds(10));

        return services;
    }

    public static IServiceCollection AdicionarTratamentoDeErros(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                if (context.HttpContext.Items.TryGetValue(CorrelationIdMiddleware.Cabecalho, out var correlationId))
                {
                    context.ProblemDetails.Extensions["correlationId"] = correlationId;
                }
            };
        });
        services.AddExceptionHandler<GlobalExceptionHandler>();
        return services;
    }
}
