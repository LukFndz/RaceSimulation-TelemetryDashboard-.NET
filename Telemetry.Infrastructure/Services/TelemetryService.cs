using Telemetry.Application.Dto;
using Telemetry.Application.Services;
using Telemetry.Infrastructure.Data;
using Telemetry.Domain.Entities;
using Microsoft.AspNetCore.SignalR;
using Telemetry.Infrastructure.Hubs;

namespace Telemetry.Infrastructure.Services;

public class TelemetryService : ITelemetryService
{
    private readonly TelemetryDbContext _db;
    private readonly IHubContext<TelemetryHub> _hub;

    public TelemetryService(TelemetryDbContext db, IHubContext<TelemetryHub> hub)
    {
        _db = db;
        _hub = hub;
    }

    public async Task ProcessTelemetryAsync(TelemetryDto dto, CancellationToken cancellationToken = default)
    {
        var entity = new TelemetrySample
        {
            Timestamp = dto.Timestamp,
            SpeedKmh = dto.SpeedKmh,
            Rpm = dto.Rpm,
            Gear = dto.Gear,
            Throttle = dto.Throttle,
            Brake = dto.Brake,
            TireTempFL = dto.TireTempFL,
            TireTempFR = dto.TireTempFR,
            TireTempRL = dto.TireTempRL,
            TireTempRR = dto.TireTempRR,
            FuelRemainingKg = dto.FuelRemainingKg,
            DrsActive = dto.DrsActive
        };

        _db.TelemetrySamples.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        await _hub.Clients.All.SendAsync("TelemetryReceived", dto, cancellationToken);

        Console.WriteLine("SignalR enviado: " + dto.SpeedKmh);
    }

    public async Task SendRaceProgressAsync(float percent, CancellationToken token)
    {
        await _hub.Clients.All.SendAsync("RaceProgress", percent, token);
    }

    public async Task SendRaceFinishedAsync(CancellationToken token)
    {
        await _hub.Clients.All.SendAsync("RaceFinished", token);
    }

}
