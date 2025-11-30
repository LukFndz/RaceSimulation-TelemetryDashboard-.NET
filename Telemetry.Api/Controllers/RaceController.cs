using Microsoft.AspNetCore.Mvc;
using Telemetry.Application.Services;

namespace Telemetry.Api.Controllers;

[ApiController]
[Route("api/race")]
public class RaceController : ControllerBase
{
    private readonly IRaceSimulationService _race;

    public RaceController(IRaceSimulationService race)
    {
        _race = race;
    }

    [HttpPost("start")]
    public async Task<IActionResult> Start([FromQuery] int laps = 5)
    {
        _ = Task.Run(() => _race.StartRaceAsync(laps));
        return Ok(new { message = "Race started", laps });

    }

    [HttpGet("status")]
    public IActionResult GetStatus()
    {
        return Ok(new { status = _race.Status.ToString() });
    }

    [HttpPost("stop")]
    public IActionResult Stop()
    {
        _race.StopRace();
        return Ok(new { message = "Race stopped" });
    }

}
