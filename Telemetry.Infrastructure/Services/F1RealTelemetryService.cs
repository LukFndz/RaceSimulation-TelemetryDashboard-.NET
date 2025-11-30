using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Text.Json;
using Telemetry.Application.Dto;
using Telemetry.Application.Services;

public class F1RealTelemetryService : IF1RealTelemetryService
{
    private readonly ITelemetryService _telemetry;
    private readonly IServiceScopeFactory _scopeFactory;

    private Process? _process;
    private CancellationTokenSource? _cts;
    private Task? _runningTask;

    private bool _running;
    public bool IsRunning => _running;

    public F1RealTelemetryService(
        ITelemetryService telemetry,
        IServiceScopeFactory scopeFactory)
    {
        _telemetry = telemetry;
        _scopeFactory = scopeFactory;
    }

    public Task StartAsync(int year, string gp, string driver)
    {
        Console.WriteLine("DEBUG: StartAsync called");

        if (_running)
        {
            Console.WriteLine("DEBUG: Already running, ignored");
            return Task.CompletedTask;
        }

        Console.WriteLine("DEBUG: STARTING REAL TELEMETRY");

        _running = true;

        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        _runningTask = Task.Run(() =>
        {
            Console.WriteLine("DEBUG: Starting RunPython...");
            return RunPython(year, gp, driver, token);
        });

        return Task.CompletedTask;
    }

    private async Task RunPython(int year, string gp, string driver, CancellationToken token)
    {
        Console.WriteLine("DEBUG: RunPython entered");

        try
        {
            var psi = new ProcessStartInfo
            {
                FileName = "py",
                Arguments = $"f1_telemetry.py {year} \"{gp}\" {driver}",
                WorkingDirectory = PathHelper.F1Data,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Console.WriteLine("DEBUG: Starting process...");
            _process = Process.Start(psi);

            if (_process == null)
            {
                Console.WriteLine("ERROR: Process failed to start");
                _running = false;
                return;
            }

            Console.WriteLine("DEBUG: Process started! Reading output...");

            while (!token.IsCancellationRequested &&
                   !_process.StandardOutput.EndOfStream)
            {
                string? line = await _process.StandardOutput.ReadLineAsync();

                if (string.IsNullOrWhiteSpace(line))
                    continue;

                Console.WriteLine($"DEBUG PY OUTPUT: {line}");

                var dto = JsonSerializer.Deserialize<TelemetryDto>(line);

                if (dto != null)
                {
                    using var scope = _scopeFactory.CreateScope();
                    var telemetry = scope.ServiceProvider.GetRequiredService<ITelemetryService>();
                    await telemetry.ProcessTelemetryAsync(dto, token);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR in RunPython: {ex}");
        }
        finally
        {
            Console.WriteLine("DEBUG: RunPython finished, cleaning...");

            _running = false;

            try { _process?.Kill(true); } catch { }
            try { _process?.Dispose(); } catch { }
        }
    }

    public void Stop()
    {
        Console.WriteLine("DEBUG: Stop called");

        Console.WriteLine($"DEBUG: States => running={_running}, cts={_cts != null}, process={_process != null}");

        if (!_running)
        {
            Console.WriteLine("DEBUG: Stop ignored because not running");
            return;
        }

        try { _cts?.Cancel(); Console.WriteLine("DEBUG: CTS cancelled"); } catch { }
        try { _process?.Kill(true); Console.WriteLine("DEBUG: Process killed"); } catch { }
        try { _process?.Dispose(); Console.WriteLine("DEBUG: Process disposed"); } catch { }

        _running = false;

        Console.WriteLine("DEBUG: Stop() completed");
    }
}
