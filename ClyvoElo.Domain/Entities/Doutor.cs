namespace ClyvoElo.Domain.Entities;

public class Doutor
{
    public int IdDoutor { get; set; }
    public int IdClinica { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Crmv { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;

    public Clinica? Clinica { get; set; }
}
