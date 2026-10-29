using ClyvoElo.Application.DTOs.Auth;

namespace ClyvoElo.Application.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginTutorAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<LoginResponse> LoginDoutorAsync(LoginRequest request, CancellationToken cancellationToken = default);
    Task<LoginResponse> CadastrarTutorAsync(CadastroTutorRequest request, CancellationToken cancellationToken = default);
}
