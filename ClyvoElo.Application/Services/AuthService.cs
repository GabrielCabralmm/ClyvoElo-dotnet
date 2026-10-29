using ClyvoElo.Application.Common;
using ClyvoElo.Application.DTOs.Auth;
using ClyvoElo.Application.Exceptions;
using ClyvoElo.Application.Interfaces;
using ClyvoElo.Application.Observabilidade;
using ClyvoElo.Domain.Entities;
using ClyvoElo.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace ClyvoElo.Application.Services;

public class AuthService : IAuthService
{
    private const string MensagemCredenciaisInvalidas = "Email ou senha inválidos.";

    private readonly ITutorRepository _tutorRepository;
    private readonly IDoutorRepository _doutorRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        ITutorRepository tutorRepository,
        IDoutorRepository doutorRepository,
        IPasswordHasher passwordHasher,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _tutorRepository = tutorRepository;
        _doutorRepository = doutorRepository;
        _passwordHasher = passwordHasher;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<LoginResponse> LoginTutorAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var tutor = await _tutorRepository.BuscarPorEmailAsync(request.Email.Trim(), cancellationToken);
        if (tutor is null || !_passwordHasher.Verificar(request.Senha, tutor.SenhaHash))
        {
            RegistrarLoginRecusado(request.Email, Papeis.Tutor);
            throw new AutenticacaoException(MensagemCredenciaisInvalidas);
        }

        _logger.LogInformation("Tutor {IdTutor} autenticado", tutor.IdTutor);
        return MontarResposta(tutor.IdTutor, tutor.Email, tutor.Nome, Papeis.Tutor);
    }

    public async Task<LoginResponse> LoginDoutorAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var doutor = await _doutorRepository.BuscarPorEmailAsync(request.Email.Trim(), cancellationToken);
        if (doutor is null || !_passwordHasher.Verificar(request.Senha, doutor.SenhaHash))
        {
            RegistrarLoginRecusado(request.Email, Papeis.Doutor);
            throw new AutenticacaoException(MensagemCredenciaisInvalidas);
        }

        _logger.LogInformation("Doutor {IdDoutor} autenticado", doutor.IdDoutor);
        return MontarResposta(doutor.IdDoutor, doutor.Email, doutor.Nome, Papeis.Doutor);
    }

    public async Task<LoginResponse> CadastrarTutorAsync(CadastroTutorRequest request, CancellationToken cancellationToken = default)
    {
        var email = request.Email.Trim();
        if (await _tutorRepository.EmailJaCadastradoAsync(email, cancellationToken))
        {
            throw new ConflitoException("Já existe uma conta com esse email.");
        }

        var tutor = new Tutor
        {
            Nome = request.Nome.Trim(),
            Email = email,
            SenhaHash = _passwordHasher.GerarHash(request.Senha),
            Telefone = request.Telefone?.Trim(),
            DataCadastro = DateTime.UtcNow,
        };

        await _tutorRepository.AdicionarAsync(tutor, cancellationToken);
        _logger.LogInformation("Novo tutor cadastrado com id {IdTutor}", tutor.IdTutor);

        return MontarResposta(tutor.IdTutor, tutor.Email, tutor.Nome, Papeis.Tutor);
    }

    private LoginResponse MontarResposta(int id, string email, string nome, string papel)
    {
        var token = _tokenService.Gerar(id, email, papel);
        return new LoginResponse
        {
            Token = token.Token,
            ExpiraEm = token.ExpiraEm,
            Papel = papel,
            Id = id,
            Nome = nome,
        };
    }

    private void RegistrarLoginRecusado(string email, string papel)
    {
        ClyvoEloTelemetry.LoginsRecusados.Add(1, new KeyValuePair<string, object?>("papel", papel));
        _logger.LogWarning("Login recusado para o email {Email} com papel {Papel}", email, papel);
    }
}
