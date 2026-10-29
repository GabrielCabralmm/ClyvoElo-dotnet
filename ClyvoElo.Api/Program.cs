using System.Text.Json.Serialization;
using ClyvoElo.Api.Extensions;
using ClyvoElo.Api.Middlewares;
using ClyvoElo.Application;
using ClyvoElo.Infrastructure;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

var portaDoRender = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrEmpty(portaDoRender))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{portaDoRender}");
}

builder.AdicionarLogEstruturado();

builder.Services
    .AddApplication()
    .AddInfrastructure(builder.Configuration)
    .AdicionarAutenticacaoJwt(builder.Configuration)
    .AdicionarDocumentacaoSwagger()
    .AdicionarObservabilidade(builder.Environment)
    .AdicionarHealthChecks()
    .AdicionarTratamentoDeErros();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .WithExposedHeaders(CorrelationIdMiddleware.Cabecalho);
    });
});

var app = builder.Build();

await app.AplicarMigracoesAsync();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseSerilogRequestLogging();

app.UseSwagger();
app.UseSwaggerUI(options => options.DocumentTitle = "CLYVO Elo API");

app.UseCors("PermitirTudo");

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();
app.MapearHealthChecks();

app.Run();

public partial class Program { }
