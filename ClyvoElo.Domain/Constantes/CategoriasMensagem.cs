namespace ClyvoElo.Domain.Constantes;

public static class CategoriasMensagem
{
    public const string PosOperatorio = "POS_OPERATORIO";
    public const string Medicacao = "MEDICACAO";
    public const string Orientacao = "ORIENTACAO";
    public const string Retorno = "RETORNO";

    public static readonly IReadOnlyCollection<string> Todas = new[] { PosOperatorio, Medicacao, Orientacao, Retorno };
}
