using ClyvoElo.Api.Extensions;
using ClyvoElo.Api.Hateoas;
using ClyvoElo.Application.Common;
using ClyvoElo.Application.DTOs.EventosClinicos;
using ClyvoElo.Application.Services;
using ClyvoElo.Domain.Filtros;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ClyvoElo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class EventosClinicosController : ControllerBase
{
    private readonly IEventoClinicoService _eventoService;

    public EventosClinicosController(IEventoClinicoService eventoService)
    {
        _eventoService = eventoService;
    }

    [HttpGet(Name = "ListarEventos")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Lista os eventos clínicos", Description = "Filtros: idPet, idDoutor, tipoEvento, status, dataInicio, dataFim. Ordenação: data, tipo, status, id (padrão: data mais recente).")]
    [ProducesResponseType(typeof(RespostaPaginada<EventoClinicoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RespostaPaginada<EventoClinicoResponse>>> Listar([FromQuery] EventoClinicoFiltro filtro, CancellationToken cancellationToken)
    {
        var resultado = await _eventoService.ListarAsync(filtro, cancellationToken);
        return Ok(resultado.ParaRespostaPaginada(Request, evento => evento.ComLinks(Url)));
    }

    [HttpGet("pet/{idPet:int}", Name = "ListarEventosDoPet")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Lista os eventos clínicos de um pet")]
    [ProducesResponseType(typeof(RespostaPaginada<EventoClinicoResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RespostaPaginada<EventoClinicoResponse>>> ListarPorPet(int idPet, [FromQuery] EventoClinicoFiltro filtro, CancellationToken cancellationToken)
    {
        filtro.IdPet = idPet;
        var resultado = await _eventoService.ListarAsync(filtro, cancellationToken);
        return Ok(resultado.ParaRespostaPaginada(Request, evento => evento.ComLinks(Url)));
    }

    [HttpGet("{id:int}", Name = "BuscarEventoPorId")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Busca um evento clínico pelo id")]
    [ProducesResponseType(typeof(EventoClinicoResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventoClinicoResponse>> BuscarPorId(int id, CancellationToken cancellationToken)
    {
        var evento = await _eventoService.BuscarPorIdAsync(id, cancellationToken);
        return Ok(evento.ComLinks(Url));
    }

    [HttpPost(Name = "CriarEvento")]
    [Authorize(Roles = Papeis.Doutor)]
    [SwaggerOperation(Summary = "Registra um evento clínico", Description = "tipoEvento: VACINA, CHECKUP, POS_OPERATORIO, MEDICACAO, EXAME. status: PENDENTE, CONCLUIDO, ATRASADO.")]
    [ProducesResponseType(typeof(EventoClinicoResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventoClinicoResponse>> Criar([FromBody] CriarEventoClinicoRequest request, CancellationToken cancellationToken)
    {
        var evento = await _eventoService.CriarAsync(request, User.ObterUsuarioLogado(), cancellationToken);
        return CreatedAtRoute("BuscarEventoPorId", new { id = evento.IdEvento }, evento.ComLinks(Url));
    }

    [HttpPut("{id:int}/status", Name = "AtualizarStatusEvento")]
    [Authorize(Roles = Papeis.Doutor)]
    [SwaggerOperation(Summary = "Atualiza o status do evento", Description = "O corpo é só o texto do status, por exemplo: \"CONCLUIDO\"")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarStatus(int id, [FromBody] string novoStatus, CancellationToken cancellationToken)
    {
        await _eventoService.AtualizarStatusAsync(id, novoStatus, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}", Name = "RemoverEvento")]
    [Authorize(Roles = Papeis.Doutor)]
    [SwaggerOperation(Summary = "Remove um evento clínico")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(int id, CancellationToken cancellationToken)
    {
        await _eventoService.RemoverAsync(id, cancellationToken);
        return NoContent();
    }
}
