namespace NetConf2024.CachingDemo.Console.Attendance;

public interface IAttendanceCalculator
{
    Task<int> Count();
}
