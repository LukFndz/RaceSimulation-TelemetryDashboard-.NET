using Telemetry.Application.Dto;

namespace Telemetry.Application.Services;

public interface ITelemetryService
{
    Task ProcessTelemetryAsync(TelemetryDto dto, CancellationToken cancellationToken = default);
    Task SendRaceProgressAsync(float percent, CancellationToken token);
    Task SendRaceFinishedAsync(CancellationToken token);
}
