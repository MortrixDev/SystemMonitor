var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<SystemMonitor.SystemMonitor>();
builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors();
app.MapGet("/stats", async (SystemMonitor.SystemMonitor monitor)  => {
	var stats = await monitor.GetSystemStats();
    return Results.Ok(stats);
});

app.Run();
