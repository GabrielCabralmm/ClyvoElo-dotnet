namespace ClyvoElo.Application.Common;

public record UsuarioLogado(int Id, string Papel)
{
    public bool EhTutor => Papel == Papeis.Tutor;
    public bool EhDoutor => Papel == Papeis.Doutor;
}
