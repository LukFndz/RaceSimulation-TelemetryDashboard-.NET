using Telemetry.Application.Services;
using Telemetry.Application.Dto;
using Telemetry.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;

namespace Telemetry.Infrastructure.Services;

public class RaceSimulationService : IRaceSimulationService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly Random _rand = new();

    private CancellationTokenSource _cts = new();
    private bool _isRunning = false;

    public RaceSimulationService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public RaceStatus Status { get; private set; } = RaceStatus.Idle;

    public async Task StartRaceAsync(int laps, CancellationToken cancellationToken = default)
    {
        if (_isRunning)
            return;

        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        Status = RaceStatus.Running;
        _isRunning = true;

        float fuel = 100f;

        for (int lap = 1; lap <= laps; lap++)
        {
            fuel = await SimulateLap(lap, laps, fuel, token);

            if (token.IsCancellationRequested)
            {
                Status = RaceStatus.Idle;
                _isRunning = false;
                return;
            }
        }

        Status = RaceStatus.Finished;
        _isRunning = false;

        // Avisar al frontend que terminó la carrera
        using var scope = _scopeFactory.CreateScope();
        var tel = scope.ServiceProvider.GetRequiredService<ITelemetryService>();
        await tel.SendRaceFinishedAsync(CancellationToken.None);
    }

    public void StopRace()
    {
        if (!_isRunning)
            return;

        _cts.Cancel();
        Status = RaceStatus.Idle;
        _isRunning = false;
    }

    private async Task<float> SimulateLap(int lapNumber, int totalLaps, float fuel, CancellationToken token)
    {
        int lapTimeMs = 10_000;      // vuelta más rápida
        int tickMs = 120;

        int elapsed = 0;

        while (elapsed < lapTimeMs)
        {
            if (token.IsCancellationRequested)
                break;

            float progress = Math.Clamp((float)elapsed / lapTimeMs, 0f, 1f);

            float totalProgress =
                ((lapNumber - 1) + progress) / totalLaps * 100f;

            using var scope = _scopeFactory.CreateScope();
            var telemetry = scope.ServiceProvider.GetRequiredService<ITelemetryService>();

            await telemetry.SendRaceProgressAsync(totalProgress, token);

            var dto = GenerateTelemetry(progress, lapNumber, totalLaps, fuel);
            fuel = dto.FuelRemainingKg;

            await telemetry.ProcessTelemetryAsync(dto, token);

            await Task.Delay(tickMs, token);
            elapsed += tickMs;
        }

        return fuel;
    }


    private TelemetryDto GenerateTelemetry(float progress, int lap, int totalLaps, float fuel)
    {
        float p = progress;

        float speed = (float)(Math.Sin(p * Math.PI) * 320) + _rand.Next(-5, 5);

        int rpm = (int)(speed * 50 + _rand.Next(-200, 200));
        rpm = Math.Clamp(rpm, 3000, 16000);

        int gear = speed switch
        {
            < 40 => 1,
            < 80 => 2,
            < 120 => 3,
            < 160 => 4,
            < 200 => 5,
            < 260 => 6,
            < 300 => 7,
            _ => 8
        };

        float throttle = (float)Math.Max(0, Math.Sin(p * Math.PI));

        float brake = 1f - throttle;
        brake = Math.Clamp(brake + (float)_rand.NextDouble() * 0.1f, 0f, 1f);

        float baseTemp = 85 + p * 15 + lap * 0.3f;

        float newFuel = fuel - 0.12f;

        bool drs = p is > 0.05f and < 0.25f;

        return new TelemetryDto(
            Timestamp: DateTime.UtcNow,
            SpeedKmh: speed,
            Rpm: rpm,
            Gear: gear,
            Throttle: throttle,
            Brake: brake,
            TireTempFL: baseTemp + _rand.Next(-2, 3),
            TireTempFR: baseTemp + _rand.Next(-2, 3),
            TireTempRL: baseTemp + _rand.Next(-2, 3),
            TireTempRR: baseTemp + _rand.Next(-2, 3),
            FuelRemainingKg: newFuel,
            DrsActive: drs
        );
    }
}
