using ClyvoElo.Api.Extensions;
using ClyvoElo.Api.Hateoas;
using ClyvoElo.Application.Common;
using ClyvoElo.Application.DTOs.Mensagens;
using ClyvoElo.Application.Services;
using ClyvoElo.Domain.Filtros;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ClyvoElo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[Authorize]
public class MensagensController : ControllerBase
{
    private readonly IMensagemAcompanhamentoService _mensagemService;

    public MensagensController(IMensagemAcompanhamentoService mensagemService)
    {
        _mensagemService = mensagemService;
    }

    [HttpGet(Name = "ListarMensagens")]
    [SwaggerOperation(Summary = "Lista as mensagens de acompanhamento (MongoDB)", Description = "O tutor só enxerga as mensagens dos próprios pets. Filtros: idPet, idDoutor, categoria, lida. Ordenação: data, assunto, categoria.")]
    [ProducesResponseType(typeof(RespostaPaginada<MensagemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<RespostaPaginada<MensagemResponse>>> Listar([FromQuery] MensagemAcompanhamentoFiltro filtro, CancellationToken cancellationToken)
    {
        var resultado = await _mensagemService.ListarAsync(filtro, User.ObterUsuarioLogado(), cancellationToken);
        return Ok(resultado.ParaRespostaPaginada(Request, mensagem => mensagem.ComLinks(Url)));
    }

    [HttpGet("{id}", Name = "BuscarMensagemPorId")]
    [SwaggerOperation(Summary = "Busca uma mensagem pelo id")]
    [ProducesResponseType(typeof(MensagemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MensagemResponse>> BuscarPorId(string id, CancellationToken cancellationToken)
    {
        var mensagem = await _mensagemService.BuscarPorIdAsync(id, User.ObterUsuarioLogado(), cancellationToken);
        return Ok(mensagem.ComLinks(Url));
    }

    [HttpPost(Name = "EnviarMensagem")]
    [Authorize(Roles = Papeis.Doutor)]
    [SwaggerOperation(Summary = "Doutor envia uma mensagem ao tutor do pet", Description = "categoria: POS_OPERATORIO, MEDICACAO, ORIENTACAO, RETORNO.")]
    [ProducesResponseType(typeof(MensagemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MensagemResponse>> Enviar([FromBody] EnviarMensagemRequest request, CancellationToken cancellationToken)
    {
        var mensagem = await _mensagemService.EnviarAsync(request, User.ObterUsuarioLogado(), cancellationToken);
        return CreatedAtRoute("BuscarMensagemPorId", new { id = mensagem.Id }, mensagem.ComLinks(Url));
    }

    [HttpPatch("{id}/lida", Name = "MarcarMensagemComoLida")]
    [Authorize(Roles = Papeis.Tutor)]
    [SwaggerOperation(Summary = "Tutor marca a mensagem como lida")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> MarcarComoLida(string id, CancellationToken cancellationToken)
    {
        await _mensagemService.MarcarComoLidaAsync(id, User.ObterUsuarioLogado(), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id}", Name = "RemoverMensagem")]
    [Authorize(Roles = Papeis.Doutor)]
    [SwaggerOperation(Summary = "Doutor remove uma mensagem que ele enviou")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Remover(string id, CancellationToken cancellationToken)
    {
        await _mensagemService.RemoverAsync(id, User.ObterUsuarioLogado(), cancellationToken);
        return NoContent();
    }
}
