using Microsoft.Extensions.Logging;

namespace CrossMath.Service.Utils.Progress;

public class LoggerProgressWriter : IProgressWriter
{
    private readonly ILogger _logger;
    
    public LoggerProgressWriter(ILogger logger) => _logger = logger;

    public void Write(string text) => _logger.LogInformation(text);
    public void WriteLine(string text) => _logger.LogInformation(text);
}