namespace CrossMath.Core.Utils
{
    public sealed class WeightedRandom<T>
    {
        private readonly (T item, int cumulative)[] _table;
        private readonly int _total;
        private readonly Random _rng = new();

        // 原版：Dictionary
        public WeightedRandom(Dictionary<T, int> weights)
            : this(weights.Select(kv => (kv.Key, kv.Value))) { }

        // 新增版：IEnumerable<(T, int)>
        public WeightedRandom(IEnumerable<(T item, int weight)> weights)
        {
            var list = new List<(T, int)>();
            int sum = 0;

            foreach (var (item, weight) in weights)
            {
                if (weight <= 0) continue;
                sum += weight;
                list.Add((item, sum));
            }

            _table = list.ToArray();
            _total = sum;
        }

        // --- Next() 不变 ---
        public T Next()
        {
            int r = _rng.Next(1, _total + 1);

            int lo = 0, hi = _table.Length - 1;
            while (lo < hi)
            {
                int mid = (lo + hi) / 2;

                if (r <= _table[mid].cumulative)
                    hi = mid;
                else
                    lo = mid + 1;
            }

            return _table[lo].item;
        }
    }
    
    public static class WeightedRandom
    {
        public static WeightedRandom<T> Create<T>(
            params (T item, int weight)[] entries)
            => new(entries);

        public static WeightedRandom<T> From<T>(
            Dictionary<T, int> dict)
            => new(dict);
    }

}


