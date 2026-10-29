namespace ClyvoElo.Application.Interfaces;

public record TokenGerado(string Token, DateTime ExpiraEm);

public interface ITokenService
{
    TokenGerado Gerar(int id, string email, string papel);
}
