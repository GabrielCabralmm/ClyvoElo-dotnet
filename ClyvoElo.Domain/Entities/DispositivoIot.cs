namespace ClyvoElo.Domain.Entities;

public class DispositivoIot
{
    public int IdDispositivo { get; set; }
    public int IdPet { get; set; }
    public string TipoDispositivo { get; set; } = string.Empty;
    public DateTime DataInstalacao { get; set; }

    public Pet? Pet { get; set; }
    public ICollection<LeituraSensor> Leituras { get; set; } = new List<LeituraSensor>();
}
