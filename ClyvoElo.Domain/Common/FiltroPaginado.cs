namespace ClyvoElo.Domain.Common;

public abstract class FiltroPaginado
{
    public const int TamanhoPaginaPadrao = 10;
    public const int TamanhoPaginaMaximo = 50;

    private int _pagina = 1;
    private int _tamanhoPagina = TamanhoPaginaPadrao;

    public int Pagina
    {
        get => _pagina;
        set => _pagina = value < 1 ? 1 : value;
    }

    public int TamanhoPagina
    {
        get => _tamanhoPagina;
        set => _tamanhoPagina = value < 1 ? TamanhoPaginaPadrao : Math.Min(value, TamanhoPaginaMaximo);
    }

    public string? OrdenarPor { get; set; }

    public string Direcao { get; set; } = "asc";

    public bool EhDescendente() => string.Equals(Direcao, "desc", StringComparison.OrdinalIgnoreCase);

    public int QuantidadeIgnorada() => (Pagina - 1) * TamanhoPagina;
}
