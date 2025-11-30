# 🏎️ F1 Telemetry Dashboard

Real-time F1-style telemetry dashboard + configurable race simulator built with C#, .NET 8, SignalR, and Chart.js.

On realgps-calculate-telemetry branch, i calculate the telemetry by last year GPs data based on FastF1 API

# 🚀 Features
# 📡 Real-time Telemetry
Speed, RPM, Gear
Throttle / Brake
Fuel
DRS
Tire temperatures

📊 Professional Charts
Real-time updates
Historical data from SQLite
Separate charts: Speed, RPM, Throttle, Brake, Fuel, Tires

# 🏁 Race Simulation
Configurable laps
Live progress & status (Idle / Running / Finished)
Stop race anytime
Persistent state on page reload

# 🧱 Architecture
Telemetry.Api – Web API + SignalR (controllers)
Telemetry.Application – DTOs, service interfaces
Telemetry.Infrastructure – EF Core, repositories, SignalR service, race simulator
Telemetry.Domain – Pure models & enums
Tech: Clean Architecture, EF Core + SQLite, SignalR

# 🖥️ Web Dashboard
Pit wall-style layout
Animated progress bar
Real-time & historical charts
CSS animations
Auto-lock buttons based on race state

# ▶️ Getting Started
# Restore packages
dotnet restore

# Run API (migrations auto-applied)
dotnet run --project Telemetry.Api


Open in browser: http://localhost:yourport/index.html

# 🏎️ Using the Simulator
Enter lap count
Click Start Race
Stop anytime with Stop Race
Graphs update in real-time
Reload page without losing state

Backend: .NET 8, ASP.NET Core Web API, SignalR, EF Core + SQLite, Clean Architecture, FastF1 API (Python)
Frontend: HTML5, CSS3, Vanilla JS, Chart.js, SignalR Client

# ❤️ Credits
Developed as an advanced practice for:
Clean Architecture
Background simulations
Real-time telemetry visualization
