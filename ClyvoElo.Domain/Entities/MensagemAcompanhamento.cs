using ClyvoElo.Domain.Constantes;
using ClyvoElo.Domain.Exceptions;

namespace ClyvoElo.Domain.Entities;

public class MensagemAcompanhamento
{
    public const int TamanhoMaximoConteudo = 2000;

    public string Id { get; set; } = string.Empty;
    public int IdPet { get; set; }
    public string NomePet { get; set; } = string.Empty;
    public int IdTutor { get; set; }
    public int IdDoutor { get; set; }
    public string NomeDoutor { get; set; } = string.Empty;
    public string Categoria { get; set; } = CategoriasMensagem.Orientacao;
    public string Assunto { get; set; } = string.Empty;
    public string Conteudo { get; set; } = string.Empty;
    public DateTime DataEnvio { get; set; }
    public bool Lida { get; set; }
    public DateTime? DataLeitura { get; set; }

    public static MensagemAcompanhamento Criar(Pet pet, Doutor doutor, string categoria, string assunto, string conteudo)
    {
        var categoriaNormalizada = categoria?.Trim().ToUpperInvariant() ?? string.Empty;
        if (!CategoriasMensagem.Todas.Contains(categoriaNormalizada))
        {
            throw new RegraDeNegocioException(
                $"Categoria inválida. Valores aceitos: {string.Join(", ", CategoriasMensagem.Todas)}.");
        }

        if (string.IsNullOrWhiteSpace(assunto))
        {
            throw new RegraDeNegocioException("O assunto da mensagem é obrigatório.");
        }

        if (string.IsNullOrWhiteSpace(conteudo))
        {
            throw new RegraDeNegocioException("O conteúdo da mensagem é obrigatório.");
        }

        if (conteudo.Length > TamanhoMaximoConteudo)
        {
            throw new RegraDeNegocioException($"A mensagem pode ter no máximo {TamanhoMaximoConteudo} caracteres.");
        }

        return new MensagemAcompanhamento
        {
            IdPet = pet.IdPet,
            NomePet = pet.Nome,
            IdTutor = pet.IdTutor,
            IdDoutor = doutor.IdDoutor,
            NomeDoutor = doutor.Nome,
            Categoria = categoriaNormalizada,
            Assunto = assunto.Trim(),
            Conteudo = conteudo.Trim(),
            DataEnvio = DateTime.UtcNow,
            Lida = false,
        };
    }

    public void MarcarComoLida()
    {
        if (Lida)
        {
            return;
        }

        Lida = true;
        DataLeitura = DateTime.UtcNow;
    }

    public bool FoiEnviadaPor(int idDoutor) => IdDoutor == idDoutor;

    public bool PertenceAoTutor(int idTutor) => IdTutor == idTutor;
}
