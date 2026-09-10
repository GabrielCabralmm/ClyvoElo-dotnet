using ClyvoElo.Api.Data;
using ClyvoElo.Api.Services;
using ClyvoElo.Api.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Serilog;
using Serilog.Context;
using Serilog.Events;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Text.Json.Serialization;
using ClyvoElo.Api.Telemetry;

var builder = WebApplication.CreateBuilder(args);

var portDoRender = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(portDoRender))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{portDoRender}");
}

builder.Host.UseSerilog((context, configuration) =>
{
    configuration
        .MinimumLevel.Information()
        .MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
        .Enrich.FromLogContext()
        .WriteTo.Console(
            outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] ({CorrelationId}) {Message:lj}{NewLine}{Exception}")
        .WriteTo.File(
            "logs/clyvoelo-.log",
            rollingInterval: RollingInterval.Day,
            outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] ({CorrelationId}) {Message:lj}{NewLine}{Exception}");
});

builder.Services.AddDbContext<ClyvoEloContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleDb")));

builder.Services.AddScoped<IPetService, PetService>();

builder.Services.AddHttpClient();

builder.Services.AddHealthChecks()
    .AddCheck<ApiSelfHealthCheck>("api")
    .AddCheck<OracleHealthCheck>("oracle_db")
    .AddCheck<ServicoExternoHealthCheck>("servico_externo");

builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(ClyvoEloTelemetry.ServiceName))
    .WithTracing(tracing => tracing
        .AddSource(ClyvoEloTelemetry.ServiceName)
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddConsoleExporter())
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddRuntimeInstrumentation()
        .AddConsoleExporter());

var jwtKey = builder.Configuration["Jwt:Key"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Cole aqui o token que o /api/Auth/login devolveu (sem escrever \"Bearer\" na frente)",
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

var app = builder.Build();

app.Use(async (httpContext, next) =>
{
    using (LogContext.PushProperty("CorrelationId", httpContext.TraceIdentifier))
    {
        await next();
    }
});

app.UseSerilogRequestLogging();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = HealthCheckResponseWriter.WriteResponse,
});

app.Run();

public partial class Program { }