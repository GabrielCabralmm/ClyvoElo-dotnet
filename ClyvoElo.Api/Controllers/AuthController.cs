using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ClyvoElo.Api.Data;
using ClyvoElo.Api.Models;

namespace ClyvoElo.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ClyvoEloContext _context;
    private readonly IConfiguration _config;
    private readonly ILogger<AuthController> _logger;

    public AuthController(ClyvoEloContext context, IConfiguration config, ILogger<AuthController> logger)
    {
        _context = context;
        _config = config;
        _logger = logger;
    }

    [HttpPost("login/tutor")]
    public async Task<IActionResult> LoginTutor([FromBody] LoginRequest request)
    {
        var tutor = await _context.Tutores.FirstOrDefaultAsync(t => t.Email == request.Email);
        if (tutor is null || !BCrypt.Net.BCrypt.Verify(request.Senha, tutor.SenhaHash))
        {
            _logger.LogWarning("Tentativa de login de tutor com credenciais inválidas para o email {Email}", request.Email);
            return Unauthorized(new { erro = "Email ou senha inválidos" });
        }

        var token = GerarToken(tutor.Email, "Tutor", tutor.IdTutor);
        return Ok(new LoginResponse { Token = token, Papel = "Tutor", Id = tutor.IdTutor, Nome = tutor.Nome });
    }

    [HttpPost("login/doutor")]
    public async Task<IActionResult> LoginDoutor([FromBody] LoginRequest request)
    {
        var doutor = await _context.Doutores.FirstOrDefaultAsync(d => d.Email == request.Email);
        if (doutor is null || !BCrypt.Net.BCrypt.Verify(request.Senha, doutor.SenhaHash))
        {
            _logger.LogWarning("Tentativa de login de doutor com credenciais inválidas para o email {Email}", request.Email);
            return Unauthorized(new { erro = "Email ou senha inválidos" });
        }

        var token = GerarToken(doutor.Email, "Doutor", doutor.IdDoutor);
        return Ok(new LoginResponse { Token = token, Papel = "Doutor", Id = doutor.IdDoutor, Nome = doutor.Nome });
    }

    private string GerarToken(string email, string papel, int id)
    {
        var jwtKey = _config["Jwt:Key"]!;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Role, papel),
            new Claim("id", id.ToString()),
        };

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
