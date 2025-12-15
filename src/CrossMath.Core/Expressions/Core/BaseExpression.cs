using System.ComponentModel.DataAnnotations;
using CrossMath.Core.Expressions.Exceptions;
namespace CrossMath.Core.Expressions.Core;

public abstract class BaseExpression : IExpression
{
    protected readonly int length;

    protected BaseExpression(int length)
    {
        this.length = length;
    }

    public int Length => length;

    // -----------------------------
    // Indexer：由子类实现 token 逻辑，
    //          Base 将提供统一的边界检查
    // -----------------------------
    public string this[int index]
    {
        get
        {
            EnsureIndex(index);
            return GetTokenCore(index);
        }
        set
        {
            EnsureIndex(index);
            SetTokenCore(index, value);
        }
    }

    protected abstract string GetTokenCore(int index);
    protected abstract void SetTokenCore(int index, string value);

    private void EnsureIndex(int index)
    {
        if (index < 0 || index >= length)
            throw new IndexOutOfRangeException($"Expression token index {index} 超出范围 {length}");
    }

    // ---------------------------------------------
    // Token 列表（统一实现）
    // ---------------------------------------------
    public List<string> GetTokens()
        => Enumerable.Range(0, length).Select(i => this[i]).ToList();

    // ---------------------------------------------
    // 工具方法：int? 解析
    // ---------------------------------------------
    protected static int? ParseNullableInt(string s)
        => int.TryParse(s, out var v) ? v : null;

    // ---------------------------------------------
    // Evaluate / Clone 必须由子类实现
    // ---------------------------------------------
    public abstract bool Evaluate();
    public abstract bool IsFullyFilled { get; }
    public abstract IExpression Clone();
    
    // =============================
    //  值语义相等性：按 Token 完全一致
    // =============================

    public bool Equals(IExpression? other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        if (Length != other.Length) return false;

        for (int i = 0; i < Length; i++)
        {
            if (this[i] != other[i])
                return false;
        }

        return true;
    }

    public override bool Equals(object? obj)
        => obj is IExpression other && Equals(other);

    // =============================
    //  Token-based HashCode（最优）
    // =============================
    public override int GetHashCode()
    {
        var hc = new HashCode();
        for (int i = 0; i < Length; i++)
            hc.Add(this[i]);
        return hc.ToHashCode();
    }
}
