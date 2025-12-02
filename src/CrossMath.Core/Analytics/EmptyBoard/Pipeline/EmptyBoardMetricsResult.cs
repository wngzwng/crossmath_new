namespace CrossMath.Core.Analytics.EmptyBoard.Pipeline;

public class BoardMetricsResult
{
    private readonly Dictionary<string, object> _dict = new();

    public void Set(string key, object value) => _dict[key] = value;
    public T Get<T>(string key) => (T)_dict[key];
    public IReadOnlyDictionary<string, object> AsReadOnly() => _dict;
}
