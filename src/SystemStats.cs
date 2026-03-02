namespace SystemMonitor;

public record SystemStats(
    long SystemTime,
	double? BatteryPercentage,
	double CpuUsagePercentage,
	double MemoryUsageMb,
	double MemoryMaxMb
);
