using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClyvoElo.Api.Data;
using ClyvoElo.Api.Models;
using ClyvoElo.Api.Telemetry;

namespace ClyvoElo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EventosClinicosController : ControllerBase
{
    private readonly ClyvoEloContext _context;
    private readonly ILogger<EventosClinicosController> _logger;

    public EventosClinicosController(ClyvoEloContext context, ILogger<EventosClinicosController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar()
    {
        var eventos = await _context.EventosClinicos
            .Include(e => e.Pet)
            .Include(e => e.Doutor)
            .OrderByDescending(e => e.DataEvento)
            .ToListAsync();
        return Ok(eventos);
    }

    [HttpGet("pet/{idPet}")]
    [AllowAnonymous]
    public async Task<IActionResult> ListarPorPet(int idPet)
    {
        var eventos = await _context.EventosClinicos
            .Include(e => e.Doutor)
            .Where(e => e.IdPet == idPet)
            .OrderByDescending(e => e.DataEvento)
            .ToListAsync();
        return Ok(eventos);
    }

    [HttpPost]
    [Authorize(Roles = "Doutor")]
    public async Task<IActionResult> Criar([FromBody] CriarEventoClinicoRequest request)
    {
        var evento = new EventoClinico
        {
            IdPet = request.IdPet,
            IdDoutor = request.IdDoutor,
            TipoEvento = request.TipoEvento,
            Descricao = request.Descricao,
            DataEvento = request.DataEvento,
            Status = request.Status,
        };

        using var activity = ClyvoEloTelemetry.ActivitySource.StartActivity("RegistrarEventoClinico");
        activity?.SetTag("evento.tipo", evento.TipoEvento);
        activity?.SetTag("evento.id_pet", evento.IdPet);
        activity?.SetTag("evento.id_doutor", evento.IdDoutor);

        _context.EventosClinicos.Add(evento);
        await _context.SaveChangesAsync();
        _logger.LogInformation(
            "Evento {TipoEvento} registrado para o pet {IdPet} pelo doutor {IdDoutor}",
            evento.TipoEvento, evento.IdPet, evento.IdDoutor);
        return CreatedAtAction(nameof(ListarPorPet), new { idPet = evento.IdPet }, evento);
    }

    [HttpPut("{id}/status")]
    [Authorize(Roles = "Doutor")]
    public async Task<IActionResult> AtualizarStatus(int id, [FromBody] string novoStatus)
    {
        var evento = await _context.EventosClinicos.FindAsync(id);
        if (evento is null)
        {
            return NotFound(new { erro = $"Evento {id} não encontrado" });
        }

        evento.Status = novoStatus;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Evento {IdEvento} atualizado para status {Status}", id, novoStatus);
        return NoContent();
    }
}
