namespace WebSocketsServer;

public class Logger
{
    private readonly object _lock = new();
    private readonly string _serverName;
    private readonly string _logFile;

    public Logger(string serverName)
    {
        _serverName = serverName;
        _logFile = $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.log";
    }
    
    public void Log(Action action, List<string> messages)
    {
        LogSingleMessage($"{_serverName} : {action}");
        foreach (var message in messages)
        {
            LogSingleMessage(message);
        }
    }
    
    public void Log(Action action, params string[] messages)
    {
        Log(action, messages.ToList());
    }
    
    private void LogSingleMessage(string message)
    {
        lock (_lock)
        {
            using var writer = new StreamWriter(_logFile, true);
            writer.WriteLine($"{DateTime.Now} : {message}");
            Console.WriteLine(message);
        }
    }

    private void LogException(Exception ex)
    {
        LogSingleMessage(ex.ToString());
    }

    private void LogExceptionWithMessage(string message, Exception ex)
    {
        LogSingleMessage(message + Environment.NewLine + ex);
    }
}