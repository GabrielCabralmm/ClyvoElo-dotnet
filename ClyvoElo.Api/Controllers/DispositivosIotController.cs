using ClyvoElo.Api.Hateoas;
using ClyvoElo.Application.DTOs.DispositivosIot;
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
public class DispositivosIotController : ControllerBase
{
    private readonly IDispositivoIotService _dispositivoService;

    public DispositivosIotController(IDispositivoIotService dispositivoService)
    {
        _dispositivoService = dispositivoService;
    }

    [HttpGet(Name = "ListarDispositivos")]
    [SwaggerOperation(Summary = "Lista os dispositivos IoT", Description = "Filtros: idPet, tipoDispositivo. Ordenação: tipo, dataInstalacao, id.")]
    [ProducesResponseType(typeof(RespostaPaginada<DispositivoIotResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RespostaPaginada<DispositivoIotResponse>>> Listar([FromQuery] DispositivoIotFiltro filtro, CancellationToken cancellationToken)
    {
        var resultado = await _dispositivoService.ListarAsync(filtro, cancellationToken);
        return Ok(resultado.ParaRespostaPaginada(Request, dispositivo => dispositivo.ComLinks(Url)));
    }

    [HttpGet("{id:int}", Name = "BuscarDispositivoPorId")]
    [SwaggerOperation(Summary = "Busca um dispositivo IoT pelo id")]
    [ProducesResponseType(typeof(DispositivoIotResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DispositivoIotResponse>> BuscarPorId(int id, CancellationToken cancellationToken)
    {
        var dispositivo = await _dispositivoService.BuscarPorIdAsync(id, cancellationToken);
        return Ok(dispositivo.ComLinks(Url));
    }

    [HttpGet("pet/{idPet:int}", Name = "ListarDispositivosDoPet")]
    [SwaggerOperation(Summary = "Lista os dispositivos IoT instalados para um pet")]
    [ProducesResponseType(typeof(RespostaPaginada<DispositivoIotResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RespostaPaginada<DispositivoIotResponse>>> ListarPorPet(int idPet, [FromQuery] DispositivoIotFiltro filtro, CancellationToken cancellationToken)
    {
        filtro.IdPet = idPet;
        var resultado = await _dispositivoService.ListarAsync(filtro, cancellationToken);
        return Ok(resultado.ParaRespostaPaginada(Request, dispositivo => dispositivo.ComLinks(Url)));
    }
}
