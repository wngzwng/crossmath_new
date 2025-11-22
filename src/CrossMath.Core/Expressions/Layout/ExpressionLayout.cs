using CrossMath.Core.Types;
using CrossMath.Core.Expressions.Schema;

namespace CrossMath.Core.Expressions.Layout;

public sealed class ExpressionLayout
{
    // 只关心“我是谁、我长什么样”
    public ExpressionId Id         { get; }
    public Direction    Direction  { get; }
    public IExpressionSchema Schema { get; }
    public IReadOnlyList<RowCol> Cells { get; }

    public int Length => Schema.Length;

    // 缓存映射（延迟构建）
    private readonly Lazy<Dictionary<RowCol, int>> _posToIndex;
    private readonly Lazy<Dictionary<int, RowCol>> _indexToPos;

    public ExpressionLayout(
        ExpressionId id,
        Direction direction,
        IEnumerable<RowCol> cells,
        IExpressionSchema? schema = null)
    {
        Id        = id;
        Direction = direction;
        Cells     = cells?.ToArray() ?? throw new ArgumentNullException(nameof(cells));
        Schema    = schema ?? ExpressionSchemaFactory.Create(Cells.Count);

        if (Schema.Length != Cells.Count)
            throw new ArgumentException("Schema length must match cell count");

        _posToIndex = new Lazy<Dictionary<RowCol, int>>(() =>
            Cells.Select((p, i) => (p, i)).ToDictionary(x => x.p, x => x.i));

        _indexToPos = new Lazy<Dictionary<int, RowCol>>(() =>
            Cells.Select((p, i) => (p, i)).ToDictionary(x => x.i, x => x.p));
    }

    // 索引器：丝滑到飞起
    public RowCol this[int index] => _indexToPos.Value[index];
    public int this[RowCol pos]   => _posToIndex.Value[pos];

    public bool TryGetIndex(RowCol pos, out int index) 
        => _posToIndex.Value.TryGetValue(pos, out index);

    public bool TryGetPosition(int index, out RowCol pos)
        => _indexToPos.Value.TryGetValue(index, out pos);

    public bool Contains(RowCol pos) => _posToIndex.Value.ContainsKey(pos);

    public IEnumerable<(RowCol Pos, CellType Type)> CellsWithTypes()
    {
        for (int i = 0; i < Length; i++)
            yield return (Cells[i], Schema.GetCellType(i));
    }

    public override string ToString()
    {
        var coords = string.Join(" ", Cells.Select(p => $"({p.Row},{p.Col})"));
        return $"{Id} | {Direction} | {Length}格 | {coords}";
    }
}