namespace business.utils;

public static class ProcessLogger
{
    private static readonly object _lockObj = new();
    private static StreamWriter? _writer;

    public static void Init(string logFilePath)
    {
        _writer = new StreamWriter(logFilePath, append: true, encoding: System.Text.Encoding.UTF8)
        {
            AutoFlush = true
        };
    }

    public static void Log(string message)
    {
        if (_writer == null) return;

        lock (_lockObj)
        {
            _writer.WriteLine($"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}");
        }
    }

    public static void Close()
    {
        lock (_lockObj)
        {
            _writer?.Close();
            _writer = null;
        }
    }
}
