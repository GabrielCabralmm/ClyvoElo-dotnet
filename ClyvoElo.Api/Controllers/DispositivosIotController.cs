using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClyvoElo.Api.Data;

namespace ClyvoElo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DispositivosIotController : ControllerBase
{
    private readonly ClyvoEloContext _context;

    public DispositivosIotController(ClyvoEloContext context)
    {
        _context = context;
    }

    [HttpGet("pet/{idPet}")]
    [AllowAnonymous]
    public async Task<IActionResult> ListarPorPet(int idPet)
    {
        var dispositivos = await _context.DispositivosIot
            .Where(d => d.IdPet == idPet)
            .ToListAsync();
        return Ok(dispositivos);
    }
}
