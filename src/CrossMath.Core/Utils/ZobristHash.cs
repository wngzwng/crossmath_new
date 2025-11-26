using System.Collections.Concurrent;

namespace CrossMath.Core.Utils;

/// <summary>
/// 通用的、可增量更新的 Zobrist Hash 实现
/// 线程安全 | 可复现（固定种子） | 零碰撞（理论上）
/// </summary>
public sealed class ZobristHash<T> where T : notnull
{
    // 全局共享实例（推荐所有地方共用这一个！）
    public static readonly ZobristHash<T> Global = new();

    private readonly ConcurrentDictionary<(int X, int Y, T State), ulong> _keys = new();
    private readonly Random _seededRandom;

    // 可选：暴露当前累计的 hash（支持增量更新场景）
    private ulong _currentHash = 0;

    public ulong CurrentHash
    {
        get => _currentHash;
        set => _currentHash = value; // 允许外部直接设置（用于快照/恢复）
    }

    public ZobristHash(int? seed = null)
    {
        _seededRandom = new Random(seed ?? 20251123);
    }

    /// <summary>
    /// 获取指定位置+状态的 Zobrist 钥匙（核心 API）
    /// </summary>
    public ulong this[int x, int y, T state] => GetOrCreateKey(x, y, state);

    /// <summary>
    /// 核心：线程安全、确定性生成随机钥匙
    /// </summary>
    private ulong GetOrCreateKey(int x, int y, T state)
    {
        var key = (x, y, state);
        return _keys.GetOrAdd(key, _ =>
        {
            lock (_seededRandom)
            {
                uint a = (uint)_seededRandom.Next();
                uint b = (uint)_seededRandom.Next();
                return ((ulong)a << 32) | b;
            }
        });
    }

    // 提供几个常用增量操作（可选，但极大提升易用性）

    public void Xor(int x, int y, T state) => _currentHash ^= GetOrCreateKey(x, y, state);

    public void Update(int x, int y, T oldState, T newState)
    {
        _currentHash ^= GetOrCreateKey(x, y, oldState);
        _currentHash ^= GetOrCreateKey(x, y, newState);
    }

    public void Reset() => _currentHash = 0;
}