using CrossMath.Core.Types;
namespace CrossMath.Core.Models;

public sealed class BoardData
{
    // 1. 核心只读属性
    public BoardLayout Layout { get; }

    public int Width  => Layout.Width;
    public int Height => Layout.Height;
    public Size BoardSize => Layout.BoardSize;

    public Dictionary<RowCol, string> FilledValues { get; private set; }
    public HashSet<RowCol> Holes { get; private set; }
    public Dictionary<RowCol, CellType> HoleTypes { get; private set; }
    public List<string> PossibleAnswers { get; private set; }

    public IEnumerable<RowCol> ValidPositions => Layout.ValidPositions();

    // 2. 最最最常用的索引器
    public string this[RowCol pos]
        => TryGetValue(pos, out var v) ? v : string.Empty;

    // 3. 构造函数 + 工厂 + 克隆（创建方式）
    public BoardData(BoardLayout layout)
    {
        Layout = layout;
        FilledValues = new();
        Holes = new(layout.ValidPositions());
        HoleTypes = Holes.ToDictionary(rc => rc, _ => CellType.Unspecified);
        PossibleAnswers = new();
    }

    public static BoardData Create(
        string layoutStr,
        int width,
        int height,
        Dictionary<RowCol, string>? filledValues = null,
        Dictionary<RowCol, CellType>? holeTypes = null,
        HashSet<RowCol>? holes = null,
        List<string>? possibleAnswers = null)
    {
        var layout = new BoardLayout(layoutStr, width, height);
        return new BoardData(layout)
        {
            FilledValues    = filledValues    ?? new(),
            HoleTypes       = holeTypes       ?? new(),
            Holes           = holes           ?? new(layout.ValidPositions()),
            PossibleAnswers = possibleAnswers ?? new(),
        };
    }

    public BoardData Clone()
    {
        return new BoardData(Layout)
        {
            FilledValues = new Dictionary<RowCol, string>(this.FilledValues),
            Holes = new HashSet<RowCol>(this.Holes),
            HoleTypes = new Dictionary<RowCol, CellType>(this.HoleTypes),
            PossibleAnswers = new List<string>(this.PossibleAnswers),
        };
    }

    // 4. 原子读写操作（核心行为，按使用频率排序）
    public bool TryGetValue(RowCol pos, out string value)
    {
        if (!Layout.IsValid(pos))
        {
            value = string.Empty;
            return false;
        }
        return FilledValues.TryGetValue(pos, out value);
    }

    public bool SetValueOnly(RowCol pos, string val)
    {
        if (!Layout.IsValid(pos)) return false;
        FilledValues[pos] = val.Trim();
        Holes.Remove(pos);
        return true;
    }

    public bool ClearValueOnly(RowCol pos)
    {
        if (!Layout.IsValid(pos)) return false;
        FilledValues.Remove(pos);
        Holes.Add(pos);
        return true;
    }

    // 5. 状态查询（轻量、常用）
    // 状态查询
    public bool IsFilled(RowCol pos) => FilledValues.ContainsKey(pos);
    
    public bool IsHole(RowCol pos)   => Holes.Contains(pos);
    
    public bool IsValid(RowCol pos)  => Layout.IsValid(pos);

    // 枚举与批量读取
    public IEnumerable<(RowCol Pos, string Value)> FilledCells()
        => FilledValues.Select(kv => (kv.Key, kv.Value));

    public IEnumerable<(RowCol Pos, CellType Type)> HolesWithType()
        => Holes.Select(p => (p, HoleTypes.GetValueOrDefault(p, CellType.Unspecified)));
    
    // 6. 类型操作（洞的类型管理）
    public bool TryGetHoleType(RowCol pos, out CellType type)
        => HoleTypes.TryGetValue(pos, out type);

    internal void AddHole(RowCol pos, CellType type)
    {
        Holes.Add(pos);
        HoleTypes[pos] = type;
    }

    internal void RemoveHole(RowCol pos)
    {
        Holes.Remove(pos);
        HoleTypes.Remove(pos);
    }

    // 7. 数据枚举（遍历相关）
    public IEnumerable<(RowCol pos, CellType type)> EnumerateHoles()
    {
        foreach (var pos in Holes)
        {
            HoleTypes.TryGetValue(pos, out var type);
            yield return (pos, type);
        }
    }

    // 8. 业务数据提取（最常用统计功能，按调用频率排序）
    public List<int> GetFilledNumbers() =>
        FilledValues.Values
            .Where(v => int.TryParse(v, out _))
            .Select(int.Parse)
            .ToList();

    public List<OpType> GetFilledOperators() =>
        FilledValues.Values
            .Where(SymbolManager.IsOperatorToken)
            .Select((string opSymbol) => SymbolManager.TryGetOpType(opSymbol, out var op) ? op : OpType.Invalid)
            .ToList();

    public List<int> GetAnswerNumbers() =>
        PossibleAnswers
            .Where(v => int.TryParse(v, out _))
            .Select(int.Parse)
            .ToList();

    public List<OpType> GetAnswerOperators() =>
        PossibleAnswers
            .Where(SymbolManager.IsOperatorToken)
            .Select((string opSymbol) => SymbolManager.TryGetOpType(opSymbol, out var op) ? op : OpType.Invalid)
            .ToList();

    public List<int> GetAllNumbers()
    {
        var list = GetFilledNumbers();
        list.AddRange(GetAnswerNumbers());
        return list;
    }

    public List<OpType> GetAllOperators()
    {
        var list = GetFilledOperators();
        list.AddRange(GetAnswerOperators());
        return list;
    }
}