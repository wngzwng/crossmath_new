using System.Collections;
using CrossMath.Core.Types;
namespace CrossMath.Core.Models;

public readonly struct BoardLayout : IEquatable<BoardLayout>, IEnumerable<RowCol>
{
    public string LayoutStr { get; }
    public int Width { get; }
    public int Height { get; }
    public int Length => LayoutStr.Length;
    
    public Size BoardSize => new(Width, Height);
    public int ValidCellCount { get; } // 新增：缓存有效格子数量，性能++

    // 索引器：支持一维和二维访问
    public string this[int index] => LayoutStr[index].ToString();
    public string this[int row, int col] => LayoutStr[row * Width + col].ToString();
    public string this[RowCol pos] => LayoutStr[pos.Row * Width + pos.Col].ToString();

    public BoardLayout(string layoutStr, Size size) : this(layoutStr, size.Width, size.Height)
    {
    }

    public BoardLayout(string layout, int width, int height)
    {
        if (layout is null) throw new ArgumentNullException(nameof(layout));
        if (width <= 0) throw new ArgumentException("Width must be positive", nameof(width));
        if (height <= 0) throw new ArgumentException("Height must be positive", nameof(height));
        if (layout.Length != width * height)
            throw new ArgumentException($"Layout length {layout.Length} != {width}×{height}");

        LayoutStr = layout;
        Width = width;
        Height = height;

        // 预计算有效格子数量（很多算法会频繁用到）
        int count = 0;
        for (int i = 0; i < layout.Length; i++)
            if (layout[i] == '1') count++;
        ValidCellCount = count;
    }

    // 超实用：直接判断坐标是否为有效格子（'1'）
    public bool IsValid(RowCol pos)
        => IsValid(pos.Row, pos.Col);

    public bool IsValid(int row, int col)
        => row >= 0 && row < Height &&
           col >= 0 && col < Width &&
           this[row, col] == "1";

    // 枚举所有有效位置（实现 IEnumerable<RowCol>）
    public Enumerator GetEnumerator() => new(this);
    IEnumerator<RowCol> IEnumerable<RowCol>.GetEnumerator() => GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // 高性能枚举器（避免 Heap 分配！）
    public struct Enumerator : IEnumerator<RowCol>
    {
        private readonly BoardLayout _layout;
        private int _index;

        public Enumerator(BoardLayout layout)
        {
            _layout = layout;
            _index = -1;
        }

        public RowCol Current => new(_index / _layout.Width, _index % _layout.Width);
        object IEnumerator.Current => Current;

        public bool MoveNext()
        {
            while (++_index < _layout.Length)
                if (_layout[_index] == "1")
                    return true;
            return false;
        }

        public void Reset() => _index = -1;
        public void Dispose() { }
    }

    // 便捷方法
    public IEnumerable<RowCol> ValidPositions() => this; // 直接 foreach 就行！

    public bool Contains(RowCol pos) => IsValid(pos);



    // ToString 调试神器
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder();
        sb.AppendLine($"BoardLayout h{Height}×w{Width} (Valid: {ValidCellCount})");
        sb.AppendLine($"Layout Str: {LayoutStr}");
        for (int r = 0; r < Height; r++)
        {
            sb.AppendLine(LayoutStr.Substring(r * Width, Width));
        }
        return sb.ToString().TrimEnd();
    }

    // -----------------------------
    // 相等性（你写得完美，已保留）
    // -----------------------------
    public bool Equals(BoardLayout other)
        => Width == other.Width &&
           Height == other.Height &&
           LayoutStr == other.LayoutStr;

    public override bool Equals(object? obj) => obj is BoardLayout other && Equals(other);

    public override int GetHashCode()
        => HashCode.Combine(LayoutStr, Width, Height);

    public static bool operator ==(BoardLayout left, BoardLayout right) => left.Equals(right);
    public static bool operator !=(BoardLayout left, BoardLayout right) => !left.Equals(right);
}

