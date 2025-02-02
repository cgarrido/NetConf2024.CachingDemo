using Microsoft.Extensions.Caching.Hybrid;

namespace NetConf2024.CachingDemo.Console.Attendance;

public class AttendanceCalculatorHybridCache(HybridCache hybridCache) : IAttendanceCalculator
{
    private const string KEY = "sexyHybridKey";

    public async Task<int> Count()
    {
        return await hybridCache.GetOrCreateAsync(
            KEY,
            cancellation => GetData()
        );
    }

    private async ValueTask<int> GetData()
    {
        await Task.Delay(2000);
        return new Random().Next(0, 100);
    }
}
