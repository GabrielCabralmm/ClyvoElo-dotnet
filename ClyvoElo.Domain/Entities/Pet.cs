using ClyvoElo.Domain.Exceptions;

namespace ClyvoElo.Domain.Entities;

public class Pet
{
    public const decimal PesoMaximoKg = 150;

    public int IdPet { get; set; }
    public int IdTutor { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Especie { get; set; } = string.Empty;
    public string? Raca { get; set; }
    public DateTime? DataNascimento { get; set; }
    public decimal? PesoKg { get; set; }

    public Tutor? Tutor { get; set; }
    public ICollection<EventoClinico> EventosClinicos { get; set; } = new List<EventoClinico>();
    public ICollection<DispositivoIot> Dispositivos { get; set; } = new List<DispositivoIot>();

    public static Pet Criar(int idTutor, string nome, string especie, string? raca, DateTime? dataNascimento, decimal? pesoKg)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new RegraDeNegocioException("O nome do pet é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(especie))
        {
            throw new RegraDeNegocioException("A espécie do pet é obrigatória.");
        }

        if (dataNascimento.HasValue && dataNascimento.Value.Date > DateTime.Today)
        {
            throw new RegraDeNegocioException("A data de nascimento não pode estar no futuro.");
        }

        var pet = new Pet
        {
            IdTutor = idTutor,
            Nome = nome.Trim(),
            Especie = especie.Trim().ToUpperInvariant(),
            Raca = raca?.Trim(),
            DataNascimento = dataNascimento,
        };

        if (pesoKg.HasValue)
        {
            pet.AtualizarPeso(pesoKg.Value);
        }

        return pet;
    }

    public void AtualizarPeso(decimal novoPeso)
    {
        if (novoPeso <= 0)
        {
            throw new RegraDeNegocioException("O peso deve ser maior que zero.");
        }

        if (novoPeso > PesoMaximoKg)
        {
            throw new RegraDeNegocioException($"O peso não pode passar de {PesoMaximoKg} kg.");
        }

        PesoKg = novoPeso;
    }

    public bool PertenceAoTutor(int idTutor) => IdTutor == idTutor;
}
