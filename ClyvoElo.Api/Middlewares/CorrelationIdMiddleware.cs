using System.Diagnostics;
using Serilog.Context;

namespace ClyvoElo.Api.Middlewares;

public class CorrelationIdMiddleware
{
    public const string Cabecalho = "X-Correlation-Id";

    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue(Cabecalho, out var valorRecebido) && !string.IsNullOrWhiteSpace(valorRecebido)
            ? valorRecebido.ToString()
            : Activity.Current?.TraceId.ToString() ?? context.TraceIdentifier;

        context.Items[Cabecalho] = correlationId;
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[Cabecalho] = correlationId;
            return Task.CompletedTask;
        });

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}
