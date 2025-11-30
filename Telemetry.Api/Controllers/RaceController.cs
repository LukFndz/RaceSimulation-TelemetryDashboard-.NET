using Microsoft.AspNetCore.Mvc;
using Telemetry.Application.Services;

[ApiController]
[Route("api/race")]
public class RaceController : ControllerBase
{
    private readonly IRaceSimulationService _race;
    private readonly IF1RealTelemetryService _real;

    public RaceController(IRaceSimulationService race, IF1RealTelemetryService real)
    {
        _race = race;
        _real = real;
    }

    [HttpPost("start")]
    public IActionResult Start([FromQuery] int laps = 5)
    {
        _ = Task.Run(() => _race.StartRaceAsync(laps));
        return Ok(new { message = "Race started", laps });
    }

    private CancellationTokenSource? _realCts;

    [HttpPost("start-real")]
    public IActionResult StartReal([FromQuery] int year, [FromQuery] string gp, [FromQuery] string driver)
    {
        _real.StartAsync(year, gp, driver);
        return Ok(new { message = "Real telemetry started" });
    }

    [HttpPost("stop")]
    public IActionResult Stop()
    {
        _real.Stop();
        _race.StopRace();
        return Ok(new { message = "Stopped" });
    }


    [HttpGet("status")]
    public IActionResult Status()
    {
        return Ok(new
        {
            virtualStatus = _race.Status.ToString(),
            realStatus = _real.IsRunning ? "Running" : "Idle"
        });
    }
}
