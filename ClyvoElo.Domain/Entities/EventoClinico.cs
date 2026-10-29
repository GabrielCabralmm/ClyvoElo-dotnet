using ClyvoElo.Domain.Constantes;
using ClyvoElo.Domain.Exceptions;

namespace ClyvoElo.Domain.Entities;

public class EventoClinico
{
    public int IdEvento { get; set; }
    public int IdPet { get; set; }
    public int IdDoutor { get; set; }
    public string TipoEvento { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public DateTime DataEvento { get; set; }
    public string Status { get; set; } = StatusEventoClinico.Pendente;

    public Pet? Pet { get; set; }
    public Doutor? Doutor { get; set; }

    public static EventoClinico Criar(int idPet, int idDoutor, string tipoEvento, string? descricao, DateTime dataEvento, string? status)
    {
        return new EventoClinico
        {
            IdPet = idPet,
            IdDoutor = idDoutor,
            TipoEvento = ValidarTipo(tipoEvento),
            Descricao = descricao?.Trim(),
            DataEvento = dataEvento,
            Status = string.IsNullOrWhiteSpace(status) ? StatusEventoClinico.Pendente : ValidarStatus(status),
        };
    }

    public void AtualizarStatus(string novoStatus)
    {
        Status = ValidarStatus(novoStatus);
    }

    private static string ValidarTipo(string tipoEvento)
    {
        var tipo = tipoEvento?.Trim().ToUpperInvariant() ?? string.Empty;
        if (!TiposEventoClinico.Todos.Contains(tipo))
        {
            throw new RegraDeNegocioException(
                $"Tipo de evento inválido. Valores aceitos: {string.Join(", ", TiposEventoClinico.Todos)}.");
        }
        return tipo;
    }

    private static string ValidarStatus(string status)
    {
        var valor = status?.Trim().ToUpperInvariant() ?? string.Empty;
        if (!StatusEventoClinico.Todos.Contains(valor))
        {
            throw new RegraDeNegocioException(
                $"Status inválido. Valores aceitos: {string.Join(", ", StatusEventoClinico.Todos)}.");
        }
        return valor;
    }
}
