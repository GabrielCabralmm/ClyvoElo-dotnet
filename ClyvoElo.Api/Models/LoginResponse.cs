namespace ClyvoElo.Api.Models;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public string Papel { get; set; } = string.Empty;
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
}
