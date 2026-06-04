namespace FinanceTracker.Api.Common;

public class ConsoleLogger : IAppLogger
{
    public void Info(string message)
    {
        Console.WriteLine($"[INFO] {DateTime.Now} : {message}");
    }
    public void Warning (string message)
    {
        Console.WriteLine($"[WARNING] {DateTime.Now} : {message}");
    }
    public void Error(string message)
    {
        Console.WriteLine($"[ERROR] {DateTime.Now} : {message}");
    }
}