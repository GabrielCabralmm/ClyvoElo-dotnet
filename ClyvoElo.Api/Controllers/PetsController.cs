using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ClyvoElo.Api.Models;
using ClyvoElo.Api.Services;

namespace ClyvoElo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PetsController : ControllerBase
{
    private readonly IPetService _petService;
    private readonly ILogger<PetsController> _logger;

    public PetsController(IPetService petService, ILogger<PetsController> logger)
    {
        _petService = petService;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar()
    {
        var pets = await _petService.ListarTodosAsync();
        return Ok(pets);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var pet = await _petService.BuscarPorIdAsync(id);
        if (pet is null)
        {
            return NotFound(new { erro = $"Pet {id} não encontrado" });
        }
        return Ok(pet);
    }

    [HttpPost]
    [Authorize(Roles = "Tutor")]
    public async Task<IActionResult> Criar([FromBody] CriarPetRequest request)
    {
        var pet = new Pet
        {
            IdTutor = request.IdTutor,
            Nome = request.Nome,
            Especie = request.Especie,
            Raca = request.Raca,
            DataNascimento = request.DataNascimento,
            PesoKg = request.PesoKg,
        };

        var criado = await _petService.CriarAsync(pet);
        _logger.LogInformation("Pet {Nome} cadastrado para o tutor {IdTutor}", criado.Nome, criado.IdTutor);
        return CreatedAtAction(nameof(BuscarPorId), new { id = criado.IdPet }, criado);
    }

    [HttpPut("{id}/peso")]
    [Authorize(Roles = "Tutor")]
    public async Task<IActionResult> AtualizarPeso(int id, [FromBody] decimal novoPeso)
    {
        try
        {
            var atualizado = await _petService.AtualizarPesoAsync(id, novoPeso);
            if (!atualizado)
            {
                return NotFound(new { erro = $"Pet {id} não encontrado" });
            }
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { erro = ex.Message });
        }
    }
}
