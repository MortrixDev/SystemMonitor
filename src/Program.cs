var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<SystemMonitor.SystemMonitor>();

var app = builder.Build();

app.MapGet("/stats", async (SystemMonitor.SystemMonitor monitor)  => {
	var stats = await monitor.GetSystemStats();
    return Results.Ok(stats);
});

app.Run();
