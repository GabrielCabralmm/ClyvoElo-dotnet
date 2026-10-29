using ClyvoElo.Application.DTOs.Auth;
using ClyvoElo.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ClyvoElo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("login/tutor")]
    [SwaggerOperation(Summary = "Login do tutor", Description = "Devolve um token JWT com o papel Tutor.")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> LoginTutor([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _authService.LoginTutorAsync(request, cancellationToken));
    }

    [HttpPost("login/doutor")]
    [SwaggerOperation(Summary = "Login do doutor", Description = "Devolve um token JWT com o papel Doutor.")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<LoginResponse>> LoginDoutor([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _authService.LoginDoutorAsync(request, cancellationToken));
    }

    [HttpPost("cadastro/tutor")]
    [SwaggerOperation(Summary = "Cadastro de tutor", Description = "Cria a conta do tutor e já devolve o token de acesso.")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<LoginResponse>> CadastroTutor([FromBody] CadastroTutorRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _authService.CadastrarTutorAsync(request, cancellationToken));
    }
}
