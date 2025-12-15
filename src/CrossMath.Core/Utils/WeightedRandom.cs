namespace CrossMath.Core.Utils
{
    public sealed class WeightedRandom<T>
    {
        private readonly (T item, int cumulative)[] _table;
        private readonly int _total;
        private readonly Random _rng = new();

        // 原版：Dictionary
        public WeightedRandom(IDictionary<T, int> weights)
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
    
    
     /// <summary>
    /// 支持任意数值类型权重的高级加权随机选择器（用于 long、float、double 等防溢出或概率场景）
    /// </summary>
    /// <typeparam name="T">项类型</typeparam>
    /// <typeparam name="TWeight">权重类型（int, long, float, double 等）</typeparam>
    public sealed class GenericWeightedRandom<T, TWeight>
        where TWeight : unmanaged, IComparable<TWeight>, IEquatable<TWeight>
    {
        private readonly (T item, TWeight cumulative)[] _table;
        private readonly TWeight _totalWeight;
        private readonly Random _rng = new();

        public GenericWeightedRandom(Dictionary<T, TWeight>? weights)
            : this(weights?.Where(kv => kv.Value.CompareTo(default!) > 0)
                         .Select(kv => (kv.Key, kv.Value))
               ?? Enumerable.Empty<(T, TWeight)>()) { }

        public GenericWeightedRandom(IDictionary<T, TWeight> weights)
            : this(weights.Where(kv => kv.Value.CompareTo(default!) > 0)
                         .Select(kv => (kv.Key, kv.Value))) { }

        public GenericWeightedRandom(IEnumerable<(T item, TWeight weight)> weights)
        {
            if (weights is null) throw new ArgumentNullException(nameof(weights));

            var list = new List<(T item, TWeight cumulative)>();
            TWeight sum = default!;

            foreach (var (item, weight) in weights)
            {
                if (weight.CompareTo(default!) <= 0 || item is null) continue;
                sum = Add(sum, weight);
                list.Add((item, sum));
            }

            if (sum.CompareTo(default!) == 0)
                throw new InvalidOperationException("Total weight must be greater than zero.");

            _table = list.ToArray();
            _totalWeight = sum;
        }

        public T Next() => NextCore(NextRandom(_totalWeight));

        private T NextCore(TWeight roll)
        {
            int low = 0, high = _table.Length - 1;
            while (low < high)
            {
                int mid = low + (high - low) / 2;
                if (_table[mid].cumulative.CompareTo(roll) >= 0)
                    high = mid;
                else
                    low = mid + 1;
            }
            return _table[low].item;
        }

        private static TWeight Add(TWeight a, TWeight b) => (dynamic)a + (dynamic)b;

        private TWeight NextRandom(TWeight exclusiveUpperBound)
        {
            dynamic max = exclusiveUpperBound;
            return typeof(TWeight) switch
            {
                var t when t == typeof(float)  => (TWeight)(object)(float)(_rng.NextDouble() * (double)max),
                var t when t == typeof(double) => (TWeight)(object)(_rng.NextDouble() * (double)max),
                _ => (TWeight)(object)_rng.NextInt64(1, Convert.ToInt64(max))
            };
        }
    }
    
    /// <summary>
    /// WeightedRandom 系列的统一静态工厂方法
    /// </summary>
    public static class WeightedRandom
    {
        // int 权重版本（推荐日常使用）
        public static WeightedRandom<T> Create<T>(params (T item, int weight)[] entries) => new(entries);
        public static WeightedRandom<T> From<T>(IDictionary<T, int> dict) => new(dict);
        public static WeightedRandom<T> From<T>(Dictionary<T, int>? dict) =>
            dict != null && dict.Any(kv => kv.Value > 0)
                ? new WeightedRandom<T>(dict)
                : throw new InvalidOperationException("No positive weights found.");

        // 自定义权重版本
        public static GenericWeightedRandom<T, TWeight> Create<T, TWeight>(params (T item, TWeight weight)[] entries)
            where TWeight : unmanaged, IComparable<TWeight>, IEquatable<TWeight>
            => new(entries);

        public static GenericWeightedRandom<T, TWeight> From<T, TWeight>(Dictionary<T, TWeight>? dict)
            where TWeight : unmanaged, IComparable<TWeight>, IEquatable<TWeight>
            => new(dict);
    }
}


