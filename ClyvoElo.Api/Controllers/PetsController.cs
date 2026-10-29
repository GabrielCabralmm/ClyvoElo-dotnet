using ClyvoElo.Api.Extensions;
using ClyvoElo.Api.Hateoas;
using ClyvoElo.Application.Common;
using ClyvoElo.Application.DTOs.Pets;
using ClyvoElo.Application.Services;
using ClyvoElo.Domain.Filtros;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ClyvoElo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class PetsController : ControllerBase
{
    private readonly IPetService _petService;

    public PetsController(IPetService petService)
    {
        _petService = petService;
    }

    [HttpGet(Name = "ListarPets")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Lista os pets", Description = "Filtros: nome, especie, raca, idTutor. Ordenação: nome, especie, peso, dataNascimento.")]
    [ProducesResponseType(typeof(RespostaPaginada<PetResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<RespostaPaginada<PetResponse>>> Listar([FromQuery] PetFiltro filtro, CancellationToken cancellationToken)
    {
        var resultado = await _petService.ListarAsync(filtro, cancellationToken);
        return Ok(resultado.ParaRespostaPaginada(Request, pet => pet.ComLinks(Url)));
    }

    [HttpGet("meus", Name = "ListarMeusPets")]
    [Authorize(Roles = Papeis.Tutor)]
    [SwaggerOperation(Summary = "Lista os pets do tutor logado")]
    [ProducesResponseType(typeof(RespostaPaginada<PetResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<RespostaPaginada<PetResponse>>> ListarMeusPets([FromQuery] PetFiltro filtro, CancellationToken cancellationToken)
    {
        var usuario = User.ObterUsuarioLogado();
        var resultado = await _petService.ListarDoTutorAsync(usuario.Id, filtro, cancellationToken);
        return Ok(resultado.ParaRespostaPaginada(Request, pet => pet.ComLinks(Url)));
    }

    [HttpGet("{id:int}", Name = "BuscarPetPorId")]
    [AllowAnonymous]
    [SwaggerOperation(Summary = "Busca um pet pelo id")]
    [ProducesResponseType(typeof(PetResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PetResponse>> BuscarPorId(int id, CancellationToken cancellationToken)
    {
        var pet = await _petService.BuscarPorIdAsync(id, cancellationToken);
        return Ok(pet.ComLinks(Url));
    }

    [HttpPost(Name = "CriarPet")]
    [Authorize(Roles = Papeis.Tutor)]
    [SwaggerOperation(Summary = "Cadastra um pet para o tutor logado", Description = "Se idTutor não for enviado, usa o tutor do token.")]
    [ProducesResponseType(typeof(PetResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<PetResponse>> Criar([FromBody] CriarPetRequest request, CancellationToken cancellationToken)
    {
        var criado = await _petService.CriarAsync(request, User.ObterUsuarioLogado(), cancellationToken);
        return CreatedAtRoute("BuscarPetPorId", new { id = criado.IdPet }, criado.ComLinks(Url));
    }

    [HttpPut("{id:int}/peso", Name = "AtualizarPesoPet")]
    [Authorize(Roles = Papeis.Tutor)]
    [SwaggerOperation(Summary = "Atualiza o peso do pet", Description = "O corpo é só o número do novo peso em kg, por exemplo: 12.5")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> AtualizarPeso(int id, [FromBody] decimal novoPeso, CancellationToken cancellationToken)
    {
        await _petService.AtualizarPesoAsync(id, novoPeso, User.ObterUsuarioLogado(), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}", Name = "RemoverPet")]
    [Authorize(Roles = Papeis.Tutor)]
    [SwaggerOperation(Summary = "Remove um pet do tutor logado")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Remover(int id, CancellationToken cancellationToken)
    {
        await _petService.RemoverAsync(id, User.ObterUsuarioLogado(), cancellationToken);
        return NoContent();
    }
}
