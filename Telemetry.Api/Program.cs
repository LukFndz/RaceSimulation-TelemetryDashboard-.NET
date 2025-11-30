using Microsoft.EntityFrameworkCore;
using Telemetry.Infrastructure.Data;
using Telemetry.Infrastructure.Services;
using Telemetry.Application.Services;
using Telemetry.Infrastructure.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<TelemetryDbContext>(options =>
    options.UseSqlite("Data Source=telemetry.db"));

builder.Services.AddSignalR();

builder.Services.AddScoped<ITelemetryService, TelemetryService>();

//builder.Services.AddHostedService<TelemetrySimulator>();

builder.Services.AddSingleton<IRaceSimulationService, RaceSimulationService>();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.UseDefaultFiles();
app.UseStaticFiles();

app.MapControllers();
app.MapHub<TelemetryHub>("/hubs/telemetry");

// Crear la DB si no existe
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TelemetryDbContext>();
    db.Database.Migrate();
}

app.Run();
