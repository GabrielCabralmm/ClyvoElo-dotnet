namespace ClyvoElo.Api.Models;

public class Clinica
{
    public int IdClinica { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string? Telefone { get; set; }

    public ICollection<Doutor> Doutores { get; set; } = new List<Doutor>();
}
