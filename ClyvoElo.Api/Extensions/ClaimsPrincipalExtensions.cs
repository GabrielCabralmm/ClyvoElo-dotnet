using System.Security.Claims;
using ClyvoElo.Application.Common;
using ClyvoElo.Application.Exceptions;

namespace ClyvoElo.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static UsuarioLogado ObterUsuarioLogado(this ClaimsPrincipal usuario)
    {
        var id = usuario.FindFirst("id")?.Value;
        var papel = usuario.FindFirst(ClaimTypes.Role)?.Value;

        if (!int.TryParse(id, out var idUsuario) || string.IsNullOrEmpty(papel))
        {
            throw new AutenticacaoException("Token sem a identificação do usuário.");
        }

        return new UsuarioLogado(idUsuario, papel);
    }
}
