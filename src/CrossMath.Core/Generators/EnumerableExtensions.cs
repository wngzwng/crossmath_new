namespace CrossMath.Core.Generators;

public static class EnumerableExtensions
{
    public static bool TryUncons<T>(
        this IEnumerable<T> source,
        out T head,
        out IEnumerable<T> tail)
    {
        if (source == null) 
            throw new ArgumentNullException(nameof(source));

        using var e = source.GetEnumerator();

        if (!e.MoveNext())
        {
            head = default!;
            tail = Array.Empty<T>();
            return false;
        }

        head = e.Current;

        // tail = 从当前位置继续往下的迭代器
        tail = EnumerateTail(e);

        return true;
    }

    private static IEnumerable<T> EnumerateTail<T>(IEnumerator<T> e)
    {
        while (e.MoveNext())
            yield return e.Current;
    }
}
