using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace ClyvoElo.Application.Observabilidade;

public static class ClyvoEloTelemetry
{
    public const string NomeServico = "ClyvoElo.Api";

    public static readonly ActivitySource ActivitySource = new(NomeServico);
    public static readonly Meter Meter = new(NomeServico);

    public static readonly Counter<long> EventosClinicosRegistrados =
        Meter.CreateCounter<long>("clyvoelo.eventos_clinicos.registrados", description: "Eventos clínicos registrados pelos doutores");

    public static readonly Counter<long> LoginsRecusados =
        Meter.CreateCounter<long>("clyvoelo.auth.logins_recusados", description: "Tentativas de login com credenciais inválidas");

    public static readonly Counter<long> MensagensEnviadas =
        Meter.CreateCounter<long>("clyvoelo.mensagens.enviadas", description: "Mensagens de acompanhamento enviadas ao tutor");
}
