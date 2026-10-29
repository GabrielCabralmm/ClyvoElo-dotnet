namespace ClyvoElo.Domain.Constantes;

public static class StatusEventoClinico
{
    public const string Pendente = "PENDENTE";
    public const string Concluido = "CONCLUIDO";
    public const string Atrasado = "ATRASADO";

    public static readonly IReadOnlyCollection<string> Todos = new[] { Pendente, Concluido, Atrasado };
}
