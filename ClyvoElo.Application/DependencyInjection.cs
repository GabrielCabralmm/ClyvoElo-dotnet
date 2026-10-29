using ClyvoElo.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ClyvoElo.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IPetService, PetService>();
        services.AddScoped<IEventoClinicoService, EventoClinicoService>();
        services.AddScoped<IDoutorService, DoutorService>();
        services.AddScoped<IDispositivoIotService, DispositivoIotService>();
        services.AddScoped<ILeituraSensorService, LeituraSensorService>();
        services.AddScoped<IMensagemAcompanhamentoService, MensagemAcompanhamentoService>();
        return services;
    }
}
