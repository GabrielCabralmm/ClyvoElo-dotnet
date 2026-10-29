using System.Text.Json;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace ClyvoElo.Api.HealthChecks;

public static class HealthCheckResponseWriter
{
    private static readonly JsonSerializerOptions Opcoes = new() { WriteIndented = true };

    public static Task WriteResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";

        var resultado = new
        {
            status = report.Status.ToString(),
            duracaoTotalMs = report.TotalDuration.TotalMilliseconds,
            checks = report.Entries.Select(e => new
            {
                nome = e.Key,
                status = e.Value.Status.ToString(),
                descricao = e.Value.Description,
                duracaoMs = e.Value.Duration.TotalMilliseconds,
                tags = e.Value.Tags,
                erro = e.Value.Exception?.Message,
            }),
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(resultado, Opcoes));
    }
}
