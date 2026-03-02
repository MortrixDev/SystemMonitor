namespace SystemMonitor;

using System.Diagnostics;
using System.Globalization;
using System.Linq;

public class SystemMonitor {
    private long _prevIdleTime;
    private long _prevTotalTime;

	public async Task<SystemStats> GetSystemStats() {
		string[] cpuLine = (await File.ReadAllTextAsync("/proc/stat"))
			.Split('\n')[0]
			.Split(' ', StringSplitOptions.RemoveEmptyEntries);

		long[] cpuTicks = cpuLine
			.Skip(1)
			.Select(long.Parse)
			.ToArray();

		long currentIdleTime = cpuTicks[3] + cpuTicks[4]; // idle + iowait
		long currentTotalTime = cpuTicks.Sum();

		double cpuUsage = 0.0;
		if (_prevTotalTime > 0) {
			long totalDiff = currentTotalTime - _prevTotalTime;
            long idleDiff = currentIdleTime - _prevIdleTime;
			cpuUsage = 100.0 * (1.0 - (double)idleDiff / totalDiff);
		}

		_prevIdleTime = currentIdleTime;
        _prevTotalTime = currentTotalTime;

		string[] memInfoLines = await File.ReadAllLinesAsync("/proc/meminfo");

		long totalMemoryKb = ParseMemInfo(memInfoLines, "MemTotal:");
		long availableMemoryKb = ParseMemInfo(memInfoLines, "MemAvailable:");
		long usedMemoryKb = totalMemoryKb - availableMemoryKb;

		double? batteryPercentage = await GetBatteryPercentage();

		return new SystemStats(
	        SystemTime: cpuTicks[2],
	        BatteryPercentage: batteryPercentage,
	        CpuUsagePercentage: Math.Round(cpuUsage, 2),
	        MemoryUsageMb: Math.Round(usedMemoryKb / 1024.0, 2),
	        MemoryMaxMb: Math.Round(totalMemoryKb / 1024.0, 2)
	    );
	}

	private long ParseMemInfo(string[] lines, string key) {
		string line = lines.First(l => l.StartsWith(key));
		if (string.IsNullOrEmpty(line)) return 0;

		return long.Parse(line.Split(' ', StringSplitOptions.RemoveEmptyEntries)[1]);
	}

	private async Task<double?> GetBatteryPercentage() {
		try {
			string powerSupplyPath = "/sys/class/power_supply/";

			var batteryDir = Directory.GetDirectories(powerSupplyPath)
				.FirstOrDefault(d => Path.GetFileName(d).StartsWith("BAT"));

			if (batteryDir != null) {
				string capacityPath = Path.Combine(batteryDir, "capacity");
				if (File.Exists(capacityPath)) {
	                string capacity = await File.ReadAllTextAsync(capacityPath);
	                return double.Parse(capacity.Trim());
	            }
			}
		} catch { }

		return null;
	}
}
