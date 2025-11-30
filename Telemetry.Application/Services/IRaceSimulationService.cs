using Telemetry.Domain.Enums;

public interface IRaceSimulationService
{
    RaceStatus Status { get; }
    Task StartRaceAsync(int laps, CancellationToken cancellationToken = default);
    void StopRace();
}
