using Microsoft.Extensions.Logging;

namespace CrossMath.Core.Utils.Progress;

public static class ProgressBarUtils
{
    private static readonly Dictionary<string, Func<IServiceProvider?, IProgressWriter>> _factories 
        = new(StringComparer.OrdinalIgnoreCase)
        {
            ["stdout"] = _ => new StdOutProgressWriter(),
            ["stderr"] = _ => new StdErrProgressWriter(),
            ["none"]   = _ => new NullProgressWriter(),

            ["logger"] = sp =>
            {
                // 如果无 DI，则降级为 stdout（避免 throw）
                if (sp == null)
                    return new StdOutProgressWriter();

                // 尝试解析 ILoggerFactory
                var loggerFactory = sp.GetService(typeof(ILoggerFactory)) as ILoggerFactory;
                if (loggerFactory == null)
                    return new StdOutProgressWriter(); // 自动降级

                var logger = loggerFactory.CreateLogger("Progress");
                return new LoggerProgressWriter(logger);
            }
        };

    /// <summary>
    /// 注册自定义 writer，例如 "file" -> FileWriter
    /// </summary>
    public static void Register(string key, Func<IServiceProvider?, IProgressWriter> creator)
        => _factories[key] = creator;

    /// <summary>
    /// 工厂创建进度条（自动选择 writer）
    /// </summary>
    public static Tqdm Create(
        int total,
        string? desc,
        string writerKey = "stdout",
        IServiceProvider? sp = null,
        int intervalMs = 50)
    {
        if (!_factories.TryGetValue(writerKey, out var factory))
            throw new ArgumentException($"Unknown progress writer key: {writerKey}");

        var writer = factory(sp);
        return new Tqdm(total, desc, intervalMs, writer);
    }
}
