using ClyvoElo.Api.Hateoas;
using ClyvoElo.Application.DTOs.Doutores;
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
public class DoutoresController : ControllerBase
{
    private readonly IDoutorService _doutorService;

    public DoutoresController(IDoutorService doutorService)
    {
        _doutorService = doutorService;
    }

    [HttpGet(Name = "ListarDoutores")]
    [SwaggerOperation(Summary = "Lista os doutores", Description = "Filtros: nome, idClinica. Ordenação: nome, crmv, id.")]
    [ProducesResponseType(typeof(RespostaPaginada<DoutorResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RespostaPaginada<DoutorResponse>>> Listar([FromQuery] DoutorFiltro filtro, CancellationToken cancellationToken)
    {
        var resultado = await _doutorService.ListarAsync(filtro, cancellationToken);
        return Ok(resultado.ParaRespostaPaginada(Request, doutor => doutor.ComLinks(Url)));
    }

    [HttpGet("{id:int}", Name = "BuscarDoutorPorId")]
    [SwaggerOperation(Summary = "Busca um doutor pelo id")]
    [ProducesResponseType(typeof(DoutorResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<DoutorResponse>> BuscarPorId(int id, CancellationToken cancellationToken)
    {
        var doutor = await _doutorService.BuscarPorIdAsync(id, cancellationToken);
        return Ok(doutor.ComLinks(Url));
    }
}
