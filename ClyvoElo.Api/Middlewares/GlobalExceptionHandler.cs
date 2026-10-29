using ClyvoElo.Application.Exceptions;
using ClyvoElo.Domain.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClyvoElo.Api.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly IProblemDetailsService _problemDetailsService;
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(IProblemDetailsService problemDetailsService, ILogger<GlobalExceptionHandler> logger)
    {
        _problemDetailsService = problemDetailsService;
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (status, titulo, detalhe) = Classificar(exception);

        if (status >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Erro não tratado em {Metodo} {Caminho}", httpContext.Request.Method, httpContext.Request.Path);
        }
        else
        {
            _logger.LogWarning("{Metodo} {Caminho} terminou com {Status}: {Mensagem}",
                httpContext.Request.Method, httpContext.Request.Path, status, detalhe);
        }

        httpContext.Response.StatusCode = status;

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = titulo,
                Detail = detalhe,
                Instance = httpContext.Request.Path,
            },
        });
    }

    private static (int Status, string Titulo, string Detalhe) Classificar(Exception exception) => exception switch
    {
        RegraDeNegocioException => (StatusCodes.Status400BadRequest, "Regra de negócio violada", exception.Message),
        AutenticacaoException => (StatusCodes.Status401Unauthorized, "Não autenticado", exception.Message),
        AcessoNegadoException => (StatusCodes.Status403Forbidden, "Acesso negado", exception.Message),
        RecursoNaoEncontradoException => (StatusCodes.Status404NotFound, "Recurso não encontrado", exception.Message),
        ConflitoException => (StatusCodes.Status409Conflict, "Conflito", exception.Message),
        DbUpdateException => (StatusCodes.Status409Conflict, "Conflito ao salvar os dados",
            "Não foi possível concluir a operação porque o registro possui vínculos com outros dados."),
        BadHttpRequestException => (StatusCodes.Status400BadRequest, "Requisição inválida", exception.Message),
        _ => (StatusCodes.Status500InternalServerError, "Erro interno no servidor",
            "Ocorreu um erro inesperado. Tente novamente mais tarde."),
    };
}
