namespace ClyvoElo.Application.Exceptions;

public class ConflitoException : Exception
{
    public ConflitoException(string mensagem) : base(mensagem)
    {
    }
}
