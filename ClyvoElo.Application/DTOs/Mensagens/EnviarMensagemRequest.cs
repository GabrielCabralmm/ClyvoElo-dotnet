using System.ComponentModel.DataAnnotations;

namespace ClyvoElo.Application.DTOs.Mensagens;

public class EnviarMensagemRequest
{
    [Range(1, int.MaxValue, ErrorMessage = "Informe o pet da mensagem.")]
    public int IdPet { get; set; }

    [Required(ErrorMessage = "A categoria é obrigatória.")]
    public string Categoria { get; set; } = string.Empty;

    [Required(ErrorMessage = "O assunto é obrigatório.")]
    [StringLength(120)]
    public string Assunto { get; set; } = string.Empty;

    [Required(ErrorMessage = "O conteúdo é obrigatório.")]
    [StringLength(2000)]
    public string Conteudo { get; set; } = string.Empty;
}
