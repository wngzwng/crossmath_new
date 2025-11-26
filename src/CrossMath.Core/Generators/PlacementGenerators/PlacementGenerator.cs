using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Expressions.Schema;
using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Types;

namespace CrossMath.Core.Generators.PlacementGenerators;

public class PlacementGenerator : IPlacementGenerator
{
    // ==========================================================
    // 0. Fields
    // ==========================================================

    private List<ExpressionLayout> _layouts = new();
    private HashSet<Placement> _filledLines = new();             // 已存在的算式线段（用于同方向冲突）
    private Dictionary<RowCol, CellType> _cellTypeMap = new();   // RowCol → CellType 映射

    private List<(int Length, CrossType Cross)> _placeStrategies = new();


    // ==========================================================
    // 1. Fluent API
    // ==========================================================

    public PlacementGenerator WithPlaceStrategies(IEnumerable<(int, CrossType)> placeStrategies)
    {
        _placeStrategies = placeStrategies.ToList();
        return this;
    }


    // ==========================================================
    // 2. 入口 API
    // ==========================================================

    public IEnumerable<Placement> Generate(ICanvas canvas)
    {
        Init(canvas);
        return GeneratePlaces(canvas, _placeStrategies);
    }

    public IEnumerable<Placement> GenerateCore(ICanvas canvas, int placeLength, CrossType crossType)
    {
        foreach (var layout in _layouts)
        {
            foreach (var p in GenerateRaw(layout, placeLength, crossType))
            {
                if (CheckAll(canvas, p, crossType))
                    yield return p;
            }
        }
    }

    private IEnumerable<Placement> GeneratePlaces(
        ICanvas canvas,
        IEnumerable<(int Length, CrossType Cross)> strategies)
    {
        foreach (var (len, cross) in strategies)
        {
            var seq = GenerateCore(canvas, len, cross);

            // var first = seq.FirstOrDefault();
            // if (first != null)
            // {
            //     yield return first;
            //     foreach (var item in seq.Skip(1))
            //         yield return item;
            // }
            
            if (seq.TryUncons(out var head, out var tail))
            {
                yield return head;
                foreach (var x in tail)
                    yield return x;
                yield break;
            }
        }
    }


    // ==========================================================
    // 3. Init（缓存耗时结果）
    // ==========================================================

    public void Init(ICanvas canvas)
    {
        _layouts = ExpressionLayoutBuilderCore
            .ExtractLayouts(canvas.CanvasSize, canvas.HasValue, [5, 7]);

        _filledLines = _layouts
            .Select(l => new Placement(
                l.Cells.First().Row,
                l.Cells.First().Col,
                l.Direction,
                l.Length))
            .ToHashSet();

        _cellTypeMap = ExpressionLayoutGraphUtils.BuildPosToCellTypeMap(_layouts);
    }


    // ==========================================================
    // 4. 原始 Placement 构造（完全不依赖 Canvas）
    // ==========================================================

    private IEnumerable<Placement> GenerateRaw(ExpressionLayout layout, int length, CrossType crossType)
    {
        var offsets   = BuildOffsets(length, layout, crossType);
        var startPos  = layout.Cells.First();
        var direction = layout.Direction.Perpendicular();

        foreach (var offset in offsets)
        {
            var pos = startPos + offset;
            yield return new Placement(pos.Row, pos.Col, direction, length);
        }
    }

    private IEnumerable<RowCol> BuildOffsets(int placeLength, ExpressionLayout mount, CrossType crossType)
    {
        var mountSchema = ExpressionSchemaFactory.Create(mount.Length);
        var placeSchema = ExpressionSchemaFactory.Create(placeLength);

        var mountDelta = mount.Direction.ToRowColDelta();
        var placeDelta = mount.Direction.Perpendicular().ToRowColDelta();

        var targetCellType = crossType switch
        {
            CrossType.Number   => CellType.Number,
            CrossType.Operator => CellType.Operator,
            CrossType.Equal    => CellType.Equal,
            _ => throw new ArgumentOutOfRangeException(nameof(crossType)),
        };

        var mountGroup = Enumerable
            .Range(0, mountSchema.Length)
            .Where(i => mountSchema.GetCellType(i) == targetCellType)
            .Select(i => i * mountDelta)
            .ToList();

        var placeGroup = Enumerable
            .Range(0, placeSchema.Length)
            .Where(i => placeSchema.GetCellType(i) == targetCellType)
            .Select(i => -i * placeDelta);

        foreach (var p in placeGroup)
        foreach (var m in mountGroup)
            yield return p + m;
    }


    // ==========================================================
    // 5. 二阶段：统一 Filter 层
    // ==========================================================

    private bool CheckAll(ICanvas canvas, Placement p, CrossType crossType)
    {
        if (FailBound(canvas, p)) return false;
        if (FailOverlap(p)) return false;
        // if (FailBreath(canvas, p, crossType)) return false; #目前只适用于单一的交叉类型
        if (FailCellType(p)) return false;
        if (FailAround(p)) return false;

        return true;
    }


    // ==========================================================
    // 6. 各种规则
    // ==========================================================

    // 6.1 越界检查 ----------------------------------------------
    private bool FailBound(ICanvas canvas, Placement p)
    {
        var delta = p.Direction.ToRowColDelta();

        var head = RowCol.At(p.Row, p.Col);
        if (!head.InBounds(canvas.CanvasSize)) return true;

        var tail = head + (p.Length - 1) * delta;
        return !tail.InBounds(canvas.CanvasSize);
    }

    // 6.2 重叠检查 ----------------------------------------------
    private bool FailOverlap(Placement p)
    {
        foreach (var existing in _filledLines)
        {
            if (existing.Direction != p.Direction) continue;
            if (existing.OverlapsWithSameDirection(p))
                return true;
        }
        return false;
    }

    // 6.3 气口检查 ----------------------------------------------
    private bool FailBreath(ICanvas canvas, Placement p, CrossType crossType)
    {
        var delta = p.Direction.ToRowColDelta();
        var perp  = p.Direction.Perpendicular().ToRowColDelta();
        var head  = RowCol.At(p.Row, p.Col);

        // (1) 首尾气口
        var prev = head - delta;
        var next = head + p.Length * delta;

        if (prev.InBounds(canvas.CanvasSize) && canvas.HasValue(prev)) return true;
        if (next.InBounds(canvas.CanvasSize) && canvas.HasValue(next)) return true;

        // (2) 中间格子的侧气口
        var schema = ExpressionSchemaFactory.Create(p.Length);

        foreach (var (pos, type) in EnumerateCellsWithType(head, delta, schema))
        {
            // 交叉点例外
            if (IsCrossPoint(type, crossType))
                continue;

            var left  = pos - perp;
            var right = pos + perp;

            if (left.InBounds(canvas.CanvasSize)  && canvas.HasValue(left))  return true;
            if (right.InBounds(canvas.CanvasSize) && canvas.HasValue(right)) return true;
        }

        return false;
    }

    private IEnumerable<(RowCol pos, CellType type)> EnumerateCellsWithType(
        RowCol head, RowCol delta, IExpressionSchema schema)
    {
        for (int i = 0; i < schema.Length; i++)
            yield return (head + i * delta, schema.GetCellType(i));
    }

    private static bool IsCrossPoint(CellType type, CrossType cross)
        => cross switch
        {
            CrossType.Operator => type == CellType.Operator,
            CrossType.Number   => type == CellType.Number,
            _ => false,
        };

    // 6.4 CellType 匹配规则 -------------------------------------
    private bool FailCellType(Placement p)
    {
        var head  = RowCol.At(p.Row, p.Col);
        var delta = p.Direction.ToRowColDelta();
        var schema = ExpressionSchemaFactory.Create(p.Length);

        for (int i = 0; i < p.Length; i++)
        {
            var pos = head + i * delta;

            if (!_cellTypeMap.TryGetValue(pos, out var expect))
                continue;

            if (schema.GetCellType(i) != expect)
                return true;
        }

        return false;
    }

    // 6.5 周边规则（未来启用） -----------------------------------
    private bool FailAround(Placement p)
    {
        var head  = RowCol.At(p.Row, p.Col);
        var delta = p.Direction.ToRowColDelta();
        var perp  = p.Direction.Perpendicular().ToRowColDelta();

        var around = new HashSet<RowCol>();

        for (int i = 0; i < p.Length; i++)
        {
            var pos = head + i * delta;
            around.Add(pos - perp);
            around.Add(pos + perp);
        }

        foreach (var l in _layouts)
        {
            if (l.Direction != p.Direction.Perpendicular())
                continue;

            if (around.Contains(l.Cells.First())) return true;
            if (around.Contains(l.Cells.Last()))  return true;
        }

        return false;
    }
}
