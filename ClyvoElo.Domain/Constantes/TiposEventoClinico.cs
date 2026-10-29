namespace ClyvoElo.Domain.Constantes;

public static class TiposEventoClinico
{
    public const string Vacina = "VACINA";
    public const string Checkup = "CHECKUP";
    public const string PosOperatorio = "POS_OPERATORIO";
    public const string Medicacao = "MEDICACAO";
    public const string Exame = "EXAME";

    public static readonly IReadOnlyCollection<string> Todos = new[] { Vacina, Checkup, PosOperatorio, Medicacao, Exame };
}
