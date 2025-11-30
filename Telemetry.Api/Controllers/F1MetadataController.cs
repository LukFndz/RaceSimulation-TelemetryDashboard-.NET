using Microsoft.AspNetCore.Mvc;
using Telemetry.Application.Services;

[ApiController]
[Route("api/f1")]
public class F1MetadataController : ControllerBase
{
    private readonly IF1MetadataService _meta;

    public F1MetadataController(IF1MetadataService meta)
    {
        _meta = meta;
    }

    [HttpGet("gps")]
    public async Task<IActionResult> GetGps([FromQuery] int year)
    {
        var list = await _meta.GetAvailableGrandsPrixAsync(year);
        return Ok(list);
    }

    [HttpGet("drivers")]
    public async Task<IActionResult> GetDrivers([FromQuery] int year, [FromQuery] string gp)
    {
        var list = await _meta.GetDriversForGpAsync(year, gp);
        return Ok(list);
    }
}
