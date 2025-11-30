using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telemetry.Application.Services
{
    public interface IF1RealTelemetryService
    {
        Task StartAsync(int year, string gp, string driver);
        void Stop();
        bool IsRunning { get; }
    }
}
