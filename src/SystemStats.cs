namespace SystemMonitor;

public record SystemStats(
    DateTime SystemTime,
	double? BatteryPercentage,
	double CpuUsagePercentage,
	double MemoryUsageMb,
	double MemoryMaxMb
);
