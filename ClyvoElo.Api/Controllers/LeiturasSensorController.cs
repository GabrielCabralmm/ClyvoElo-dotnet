using ClyvoElo.Api.Hateoas;
using ClyvoElo.Application.DTOs.LeiturasSensor;
using ClyvoElo.Application.Services;
using ClyvoElo.Domain.Filtros;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ClyvoElo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
[AllowAnonymous]
public class LeiturasSensorController : ControllerBase
{
    private readonly ILeituraSensorService _leituraService;

    public LeiturasSensorController(ILeituraSensorService leituraService)
    {
        _leituraService = leituraService;
    }

    [HttpGet(Name = "ListarLeituras")]
    [SwaggerOperation(Summary = "Lista as leituras dos sensores", Description = "Filtros: idPet, idDispositivo, tipoMedida, dataInicio, dataFim. Ordenação: data, valor, tipo (padrão: mais recente).")]
    [ProducesResponseType(typeof(RespostaPaginada<LeituraSensorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RespostaPaginada<LeituraSensorResponse>>> Listar([FromQuery] LeituraSensorFiltro filtro, CancellationToken cancellationToken)
    {
        var resultado = await _leituraService.ListarAsync(filtro, cancellationToken);
        return Ok(resultado.ParaRespostaPaginada(Request, leitura => leitura.ComLinks(Url)));
    }

    [HttpGet("{id:int}", Name = "BuscarLeituraPorId")]
    [SwaggerOperation(Summary = "Busca uma leitura pelo id")]
    [ProducesResponseType(typeof(LeituraSensorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeituraSensorResponse>> BuscarPorId(int id, CancellationToken cancellationToken)
    {
        var leitura = await _leituraService.BuscarPorIdAsync(id, cancellationToken);
        return Ok(leitura.ComLinks(Url));
    }

    [HttpGet("pet/{idPet:int}", Name = "ListarLeiturasDoPet")]
    [SwaggerOperation(Summary = "Lista as leituras de todos os dispositivos de um pet")]
    [ProducesResponseType(typeof(RespostaPaginada<LeituraSensorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RespostaPaginada<LeituraSensorResponse>>> ListarPorPet(int idPet, [FromQuery] LeituraSensorFiltro filtro, CancellationToken cancellationToken)
    {
        filtro.IdPet = idPet;
        var resultado = await _leituraService.ListarAsync(filtro, cancellationToken);
        return Ok(resultado.ParaRespostaPaginada(Request, leitura => leitura.ComLinks(Url)));
    }

    [HttpGet("dispositivo/{idDispositivo:int}", Name = "ListarLeiturasDoDispositivo")]
    [SwaggerOperation(Summary = "Lista as leituras de um dispositivo")]
    [ProducesResponseType(typeof(RespostaPaginada<LeituraSensorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RespostaPaginada<LeituraSensorResponse>>> ListarPorDispositivo(int idDispositivo, [FromQuery] LeituraSensorFiltro filtro, CancellationToken cancellationToken)
    {
        filtro.IdDispositivo = idDispositivo;
        var resultado = await _leituraService.ListarAsync(filtro, cancellationToken);
        return Ok(resultado.ParaRespostaPaginada(Request, leitura => leitura.ComLinks(Url)));
    }

    [HttpPost(Name = "RegistrarLeitura")]
    [SwaggerOperation(Summary = "Registra uma leitura enviada pelo dispositivo IoT")]
    [ProducesResponseType(typeof(LeituraSensorResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<LeituraSensorResponse>> Registrar([FromBody] RegistrarLeituraRequest request, CancellationToken cancellationToken)
    {
        var leitura = await _leituraService.RegistrarAsync(request, cancellationToken);
        return CreatedAtRoute("BuscarLeituraPorId", new { id = leitura.IdLeitura }, leitura.ComLinks(Url));
    }
}
