using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetConf2024.CachingDemo.Console.Attendance;
using System.Diagnostics;

const int NUM_ITERS = 5;

// Create a host to manage dependencies
using IHost host = Host.CreateDefaultBuilder()
    .ConfigureServices((_, services) =>
    {
        services.AddMemoryCache();

        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "127.0.0.1:6379"; // Change if running Redis in Docker
            options.InstanceName = "SampleInstance";
        });
#pragma warning disable EXTEXP0018 // experimental (pre-release)
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new()
            {
                LocalCacheExpiration = TimeSpan.FromMinutes(20),
                Expiration = TimeSpan.FromMinutes(20),
            };
        });
#pragma warning restore EXTEXP0018

        services.AddKeyedScoped<IAttendanceCalculator, AttendanceCalculatorWithoutCache>(AttendanceCalculatorType.WITHOUT_CACHE);
        services.AddKeyedScoped<IAttendanceCalculator, AttendanceCalculatorMemoryCache>(AttendanceCalculatorType.MEMORY_CACHE);
        services.AddKeyedScoped<IAttendanceCalculator, AttendanceCalculatorDistributedCache>(AttendanceCalculatorType.DISTRIBUTED_CACHE);
        services.AddKeyedScoped<IAttendanceCalculator, AttendanceCalculatorHybridCache>(AttendanceCalculatorType.HYBRID_CACHE);
    })
    .Build();

await Calculate(AttendanceCalculatorType.WITHOUT_CACHE);
await Calculate(AttendanceCalculatorType.MEMORY_CACHE);
await Calculate(AttendanceCalculatorType.DISTRIBUTED_CACHE);
await Calculate(AttendanceCalculatorType.HYBRID_CACHE);

async Task Calculate(AttendanceCalculatorType type)
{
    var attendanceCalculator = host.Services.GetRequiredKeyedService<IAttendanceCalculator>(type);
    var stopwatch = new Stopwatch();
    var executionTimes = new List<long>();

    for (int i = 0; i < NUM_ITERS; i++)
    {
        stopwatch.Restart();
        _ = await attendanceCalculator.Count();
        stopwatch.Stop();

        executionTimes.Add(stopwatch.ElapsedMilliseconds);
    }

    // Calculate statistics
    var minTime = executionTimes.Min();
    var maxTime = executionTimes.Max();
    var avgTime = executionTimes.Average();

    //Print
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write($"{type} ");
    Console.ResetColor();
    Console.WriteLine($"- Min: {minTime} ms, Max: {maxTime} ms, Avg: {avgTime:F2} ms");
}


