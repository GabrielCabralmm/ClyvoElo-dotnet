using ClyvoElo.Application.Common;

namespace ClyvoElo.Application.DTOs.Mensagens;

public class MensagemResponse : RecursoHateoas
{
    public string Id { get; set; } = string.Empty;
    public int IdPet { get; set; }
    public string NomePet { get; set; } = string.Empty;
    public int IdTutor { get; set; }
    public int IdDoutor { get; set; }
    public string NomeDoutor { get; set; } = string.Empty;
    public string Categoria { get; set; } = string.Empty;
    public string Assunto { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public DateTime DataEnvio { get; set; }
    public bool Lida { get; set; }
    public DateTime? DataLeitura { get; set; }
}
