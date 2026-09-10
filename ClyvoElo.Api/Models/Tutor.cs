namespace ClyvoElo.Api.Models;

public class Tutor
{
    public int IdTutor { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string? Telefone { get; set; }
    public DateTime DataCadastro { get; set; }

    public ICollection<Pet> Pets { get; set; } = new List<Pet>();
}
