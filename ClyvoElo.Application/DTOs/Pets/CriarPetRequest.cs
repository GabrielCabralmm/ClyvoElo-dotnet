using System.ComponentModel.DataAnnotations;

namespace ClyvoElo.Application.DTOs.Pets;

public class CriarPetRequest
{
    public int? IdTutor { get; set; }

    [Required(ErrorMessage = "O nome do pet é obrigatório.")]
    [StringLength(100)]
    public string Nome { get; set; } = string.Empty;

    [Required(ErrorMessage = "A espécie é obrigatória.")]
    [StringLength(50)]
    public string Especie { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Raca { get; set; }

    public DateTime? DataNascimento { get; set; }

    [Range(0.01, 150, ErrorMessage = "O peso deve estar entre 0,01 e 150 kg.")]
    public decimal? PesoKg { get; set; }
}
