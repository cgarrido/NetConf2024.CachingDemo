using Microsoft.Extensions.Caching.Memory;

namespace NetConf2024.CachingDemo.Console.Attendance;

public class AttendanceCalculatorMemoryCache(IMemoryCache memoryCache) : IAttendanceCalculator
{
    private const string KEY = "sexyKey";

    public async Task<int> Count()
    {
        if (!(memoryCache.TryGetValue(KEY, out int? value)
            && value.HasValue))
        {
            await Task.Delay(2000);
            value = new Random().Next(0, 100);
            memoryCache.Set(KEY, value);
        }

        return value.Value;
    }
}
