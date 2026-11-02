using ClyvoElo.Application.Common;
using ClyvoElo.Application.DTOs.Alertas;
using ClyvoElo.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ClyvoElo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize(Roles = Papeis.Doutor)]
public class AlertasController : ControllerBase
{
    private readonly IAlertaService _alertaService;

    public AlertasController(IAlertaService alertaService)
    {
        _alertaService = alertaService;
    }

    [HttpPost(Name = "RegistrarAlerta")]
    [SwaggerOperation(
        Summary = "Registra um alerta (pkg_clyvo_alerta.prc_registrar_alerta)",
        Description = "origem: CLINICO ou AMBIENTAL. prioridade: BAIXA, MEDIA ou ALTA. A pontuação de risco é calculada no Oracle pela função fn_calcula_pontuacao_risco (somaFatores / qtdFatores, limitada a 10). A trigger trg_auditoria_alerta grava o INSERT na tabela AUDITORIA.")]
    [ProducesResponseType(typeof(AlertaRegistradoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AlertaRegistradoResponse>> Registrar([FromBody] RegistrarAlertaRequest request, CancellationToken cancellationToken)
    {
        var alerta = await _alertaService.RegistrarAsync(request, cancellationToken);
        return StatusCode(StatusCodes.Status201Created, alerta);
    }

    [HttpPut("{id:int}/status", Name = "AtualizarStatusAlerta")]
    [SwaggerOperation(
        Summary = "Atualiza o status do alerta (pkg_clyvo_alerta.prc_atualizar_status)",
        Description = "O corpo é só o novo status entre aspas: \"NOVO\", \"ENVIADO\" ou \"RESOLVIDO\". A trigger grava o UPDATE na AUDITORIA.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarStatus(int id, [FromBody] string status, CancellationToken cancellationToken)
    {
        await _alertaService.AtualizarStatusAsync(id, status, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}", Name = "ExcluirAlerta")]
    [SwaggerOperation(
        Summary = "Exclui um alerta (pkg_clyvo_alerta.prc_excluir_alerta)",
        Description = "A trigger grava o DELETE na AUDITORIA com os valores antigos do alerta.")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Excluir(int id, CancellationToken cancellationToken)
    {
        await _alertaService.ExcluirAsync(id, cancellationToken);
        return NoContent();
    }

    [HttpGet("relatorio", Name = "RelatorioAlertas")]
    [Produces("text/plain")]
    [SwaggerOperation(
        Summary = "Relatório de pontuação por origem e prioridade (pkg_clyvo_alerta.prc_relatorio_alertas)",
        Description = "Soma manual por origem/prioridade, subtotal por origem e total geral. Filtro opcional: origem (CLINICO ou AMBIENTAL).")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Relatorio([FromQuery] string? origem, CancellationToken cancellationToken)
    {
        var relatorio = await _alertaService.GerarRelatorioAsync(origem, cancellationToken);
        return Content(relatorio, "text/plain; charset=utf-8");
    }
}
