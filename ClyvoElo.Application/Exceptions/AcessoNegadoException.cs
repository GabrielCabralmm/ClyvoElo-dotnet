namespace ClyvoElo.Application.Exceptions;

public class AcessoNegadoException : Exception
{
    public AcessoNegadoException(string mensagem) : base(mensagem)
    {
    }
}
