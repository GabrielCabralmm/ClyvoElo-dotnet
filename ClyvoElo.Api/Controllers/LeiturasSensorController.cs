using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClyvoElo.Api.Data;
using ClyvoElo.Api.Models;

namespace ClyvoElo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeiturasSensorController : ControllerBase
{
    private readonly ClyvoEloContext _context;
    private readonly ILogger<LeiturasSensorController> _logger;

    public LeiturasSensorController(ClyvoEloContext context, ILogger<LeiturasSensorController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("pet/{idPet}")]
    [AllowAnonymous]
    public async Task<IActionResult> ListarPorPet(int idPet)
    {
        var leituras = await _context.LeiturasSensor
            .Include(l => l.Dispositivo)
            .Where(l => l.Dispositivo != null && l.Dispositivo.IdPet == idPet)
            .OrderByDescending(l => l.DataHoraLeitura)
            .ToListAsync();
        return Ok(leituras);
    }

    [HttpGet("dispositivo/{idDispositivo}")]
    [AllowAnonymous]
    public async Task<IActionResult> ListarPorDispositivo(int idDispositivo)
    {
        var leituras = await _context.LeiturasSensor
            .Where(l => l.IdDispositivo == idDispositivo)
            .OrderByDescending(l => l.DataHoraLeitura)
            .ToListAsync();
        return Ok(leituras);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Registrar([FromBody] LeituraSensor leitura)
    {
        _context.LeiturasSensor.Add(leitura);
        await _context.SaveChangesAsync();
        _logger.LogInformation(
            "Leitura {TipoMedida} = {ValorMedido} registrada para o dispositivo {IdDispositivo}",
            leitura.TipoMedida, leitura.ValorMedido, leitura.IdDispositivo);
        return CreatedAtAction(nameof(ListarPorDispositivo), new { idDispositivo = leitura.IdDispositivo }, leitura);
    }
}
