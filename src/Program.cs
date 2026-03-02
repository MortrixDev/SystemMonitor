var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<SystemMonitor>();

var app = builder.Build();

// Map a simple endpoint to trigger the monitor logic
app.MapGet("/stats", (SystemMonitor monitor) => monitor.GetSystemStats());

app.Run();
