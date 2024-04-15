using Microsoft.Extensions.Logging;

namespace JustProxies.Test;

public class ConsoleLogger<T> : ILogger<T>
{
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state,
        Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Console.WriteLine($"[{DateTime.Now:yyyy-MM-dd HH:mm:ss,fff}][{logLevel}] {state} {exception?.Message}");
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return true;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null!;
    }
}