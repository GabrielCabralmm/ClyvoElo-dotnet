using System.Diagnostics;

namespace ClyvoElo.Api.Telemetry;

public static class ClyvoEloTelemetry
{
    public const string ServiceName = "ClyvoElo.Api";
    public static readonly ActivitySource ActivitySource = new(ServiceName);
}
