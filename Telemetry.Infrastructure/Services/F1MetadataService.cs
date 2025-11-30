using System.Diagnostics;
using Telemetry.Application.Services;

namespace Telemetry.Infrastructure.Services
{
    public class F1MetadataService : IF1MetadataService
    {
        private async Task<List<string>> RunPythonListCommand(string args)
        {
            var psi = new ProcessStartInfo
            {
                FileName = "py",
                Arguments = $"f1_metadata.py {args}",
                WorkingDirectory = PathHelper.F1Data,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            var p = Process.Start(psi);

            var results = new List<string>();

            while (!p.StandardOutput.EndOfStream)
            {
                var line = await p.StandardOutput.ReadLineAsync();
                if (!string.IsNullOrWhiteSpace(line))
                    results.Add(line.Trim());
            }

            return results;
        }

        public Task<List<string>> GetAvailableGrandsPrixAsync(int year)
            => RunPythonListCommand($"gp-list {year}");

        public Task<List<string>> GetDriversForGpAsync(int year, string gp)
            => RunPythonListCommand($"driver-list {year} \"{gp}\"");
    }
}