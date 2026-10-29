using ClyvoElo.Api.Middlewares;
using ClyvoElo.Application.Exceptions;
using ClyvoElo.Domain.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;

namespace ClyvoElo.Tests.Unit.Api;

public class GlobalExceptionHandlerTests
{
    private readonly ProblemDetailsServiceFalso _problemDetailsService = new();
    private readonly GlobalExceptionHandler _handler;

    public GlobalExceptionHandlerTests()
    {
        _handler = new GlobalExceptionHandler(_problemDetailsService, NullLogger<GlobalExceptionHandler>.Instance);
    }

    public static TheoryData<Exception, int> ExcecoesEStatus => new()
    {
        { new RegraDeNegocioException("peso inválido"), StatusCodes.Status400BadRequest },
        { new AutenticacaoException("credenciais"), StatusCodes.Status401Unauthorized },
        { new AcessoNegadoException("outro tutor"), StatusCodes.Status403Forbidden },
        { new RecursoNaoEncontradoException("pet 9"), StatusCodes.Status404NotFound },
        { new ConflitoException("email"), StatusCodes.Status409Conflict },
        { new InvalidOperationException("falha inesperada"), StatusCodes.Status500InternalServerError },
    };

    [Theory]
    [MemberData(nameof(ExcecoesEStatus))]
    public async Task TryHandleAsync_TipoDeExcecao_DevolveStatusHttpCorrespondente(Exception excecao, int statusEsperado)
    {
        var httpContext = new DefaultHttpContext();

        var tratou = await _handler.TryHandleAsync(httpContext, excecao, CancellationToken.None);

        Assert.True(tratou);
        Assert.Equal(statusEsperado, httpContext.Response.StatusCode);
        Assert.Equal(statusEsperado, _problemDetailsService.UltimoContexto!.ProblemDetails.Status);
    }

    [Fact]
    public async Task TryHandleAsync_ErroInterno_NaoExpoeMensagemOriginal()
    {
        await _handler.TryHandleAsync(new DefaultHttpContext(), new Exception("ORA-12541 senha do banco"), CancellationToken.None);

        Assert.DoesNotContain("ORA-12541", _problemDetailsService.UltimoContexto!.ProblemDetails.Detail);
    }

    [Fact]
    public async Task TryHandleAsync_RegraDeNegocio_UsaMensagemDaRegraNoDetalhe()
    {
        await _handler.TryHandleAsync(new DefaultHttpContext(), new RegraDeNegocioException("O peso deve ser maior que zero."), CancellationToken.None);

        Assert.Equal("O peso deve ser maior que zero.", _problemDetailsService.UltimoContexto!.ProblemDetails.Detail);
    }

    private class ProblemDetailsServiceFalso : IProblemDetailsService
    {
        public ProblemDetailsContext? UltimoContexto { get; private set; }

        public ValueTask WriteAsync(ProblemDetailsContext context)
        {
            UltimoContexto = context;
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> TryWriteAsync(ProblemDetailsContext context)
        {
            UltimoContexto = context;
            return ValueTask.FromResult(true);
        }
    }
}
