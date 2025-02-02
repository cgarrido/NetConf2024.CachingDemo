namespace NetConf2024.CachingDemo.Console.Attendance;

public class AttendanceCalculatorWithoutCache : IAttendanceCalculator
{
    public async Task<int> Count()
    {
        await Task.Delay(2000);
        return new Random().Next(0, 100);
    }
}
