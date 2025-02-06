using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace NetConf2024.CachingDemo.Console.Attendance;

public class AttendanceCalculatorDistributedCache(IDistributedCache distributedCache) : IAttendanceCalculator
{
    private const string KEY = "sexyKey";

    public async Task<int> Count()
    {
        var cachedArr = await distributedCache.GetAsync(KEY);
        int value;

        if (cachedArr is null)
        {
            await Task.Delay(2000);
            value = new Random().Next(0, 100);
            cachedArr = JsonSerializer.SerializeToUtf8Bytes(value);
            await distributedCache.SetAsync(KEY, cachedArr, new() { AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(20) });
            return value;
        }
        else
            return JsonSerializer.Deserialize<int>(cachedArr);
    }
}
