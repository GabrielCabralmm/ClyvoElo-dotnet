using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ClyvoElo.Api.Data;

namespace ClyvoElo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DoutoresController : ControllerBase
{
    private readonly ClyvoEloContext _context;

    public DoutoresController(ClyvoEloContext context)
    {
        _context = context;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Listar()
    {
        var doutores = await _context.Doutores.Include(d => d.Clinica).ToListAsync();
        return Ok(doutores);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> BuscarPorId(int id)
    {
        var doutor = await _context.Doutores.Include(d => d.Clinica)
            .FirstOrDefaultAsync(d => d.IdDoutor == id);
        if (doutor is null)
        {
            return NotFound(new { erro = $"Doutor {id} não encontrado" });
        }
        return Ok(doutor);
    }
}
