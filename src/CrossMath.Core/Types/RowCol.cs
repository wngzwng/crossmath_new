namespace CrossMath.Core.Types;

public readonly record struct RowCol(int Row, int Col):IComparable<RowCol>
{
    public static RowCol Zero => new(0, 0);
    public static RowCol One  => new(1, 1);

    // 四个方向常量（配合 Direction 使用更丝滑）
    public static RowCol Up    => new(-1, 0);
    public static RowCol Down  => new( 1, 0);
    public static RowCol Left  => new( 0,-1);
    public static RowCol Right => new( 0, 1);

    // 常用属性
    public bool IsOrigin => Row == 0 && Col == 0;

    // 1. 静态工厂方法（最优雅！强烈推荐） 
    public static RowCol At(int row, int col) => new(row, col);
    // 运算符
    public static RowCol operator +(RowCol a, RowCol b) => new(a.Row + b.Row, a.Col + b.Col);
    public static RowCol operator -(RowCol a, RowCol b) => new(a.Row - b.Row, a.Col - b.Col);
    public static RowCol operator *(RowCol a, int scalar) => new(a.Row * scalar, a.Col * scalar); 
    public static RowCol operator *(int scalar, RowCol a) => a * scalar; public static RowCol operator /(RowCol a, int scalar) => new(a.Row / scalar, a.Col / scalar);


    // 曼哈顿距离 & 切比雪夫距离
    public int ManhattanDistance(RowCol other)
        => Math.Abs(Row - other.Row) + Math.Abs(Col - other.Col);

    // 解构支持（foreach 直接拆）
    public void Deconstruct(out int r, out int c) => (r, c) = (Row, Col);

    // 实现IComparable接口 - 支持排序！
    public int CompareTo(RowCol other)
    {
        // 先按Row排序，再按Col排序（自然顺序）
        int rowComparison = Row.CompareTo(other.Row);
        if (rowComparison != 0)
            return rowComparison;
        
        return Col.CompareTo(other.Col);
    }
    public override string ToString() => $"({Row},{Col})";
    public string ToString(string format) => $"({Row.ToString(format)},{Col.ToString(format)})";
}