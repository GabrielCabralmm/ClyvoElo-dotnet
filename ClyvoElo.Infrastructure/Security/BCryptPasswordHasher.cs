using ClyvoElo.Application.Interfaces;

namespace ClyvoElo.Infrastructure.Security;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string GerarHash(string senha) => BCrypt.Net.BCrypt.HashPassword(senha);

    public bool Verificar(string senha, string hash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(senha, hash);
        }
        catch (BCrypt.Net.SaltParseException)
        {
            return false;
        }
    }
}
