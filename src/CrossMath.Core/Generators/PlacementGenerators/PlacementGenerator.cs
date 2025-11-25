using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Expressions.Schema;
using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Types;

namespace CrossMath.Core.Generators.PlacementGenerators;

public class PlacementGenerator : IPlacementGenerator
{
    private List<ExpressionLayout> _layouts = new();
    private HashSet<Placement> _filledLines = new();        // 已占用的算式线段（同方向冲突）
    private Dictionary<RowCol, CellType> _cellTypeMap = new(); // 每个 RowCol 已知的 CellType

    // ==========================================================
    // 1. 入口：Generate
    // ==========================================================
    public IEnumerable<Placement> Generate(
        ICanvas canvas,
        int placeLength,
        CrossType crossType)
    {
        Init(canvas);

        foreach (var layout in _layouts)
        {
            foreach (var p in GenerateCore(canvas, placeLength, crossType, layout))
                yield return p;
        }
    }

    public IEnumerable<Placement> GenerateCore(
        ICanvas canvas,
        int placeLength,
        CrossType crossType,
        ExpressionLayout layout)
    {
        return GenerateRaw(layout, placeLength, crossType)
            .Where(p => CheckAll(canvas, p, crossType));
    }

    // ==========================================================
    // 1.1 Init — 提前缓存所有静态信息
    // ==========================================================
    public void Init(ICanvas canvas)
    {
        _layouts = ExpressionLayoutBuilderCore
            .ExtractLayouts(canvas.CanvasSize, canvas.HasValue, [5, 7]);

        // 记录所有已存在的表达式的起点 (用于 FailOverlap)
        _filledLines = _layouts
            .Select(l => new Placement(
                l.Cells.First().Row,
                l.Cells.First().Col,
                l.Direction,
                l.Length))
            .ToHashSet();

        // 用表达式图生成 (RowCol → CellType) 映射
        _cellTypeMap = ExpressionLayoutGraphUtils.BuildPosToCellTypeMap(_layouts);
    }

    // ==========================================================
    // 2. 原始 Placement 生成（只依赖 layout）
    // ==========================================================
    private IEnumerable<Placement> GenerateRaw(ExpressionLayout mount, int placeLength, CrossType crossType)
    {
        var offsets = BuildOffsets(placeLength, mount, crossType);
        var startPos = mount.Cells.First();
        var direction = mount.Direction.Perpendicular();

        foreach (var offset in offsets)
        {
            var pos = startPos + offset;
            yield return new Placement(pos.Row, pos.Col, direction, placeLength);
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
            CrossType.Number => CellType.Number,
            CrossType.Operator => CellType.Operator,
            CrossType.Equal => CellType.Equal,
            _ => throw new ArgumentOutOfRangeException(nameof(crossType), crossType, null),
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
    // 3. Filter (依次执行所有检查)
    // ==========================================================
    public bool CheckAll(ICanvas canvas, Placement p, CrossType crossType)
    {
        if (FailBound(canvas, p)) return false;
        if (FailOverlap(p)) return false;
        if (FailBreath(canvas, p, crossType)) return false;
        if (FailCellType(p)) return false;
        // if (FailAround(p)) return false;

        return true;
    }

    // ==========================================================
    // 3.1 越界
    // ==========================================================
    public bool FailBound(ICanvas canvas, Placement p)
    {
        var delta = p.Direction.ToRowColDelta();

        var head = RowCol.At(p.Row, p.Col);
        if (!head.InBounds(canvas.CanvasSize)) return true;

        var tail = head + (p.Length - 1) * delta;
        return !tail.InBounds(canvas.CanvasSize);
    }

    // ==========================================================
    // 3.2 同方向重叠（基于 filledLines）
    // ==========================================================
    public bool FailOverlap(Placement p)
    {
        foreach (var existing in _filledLines)
        {
            if (existing.Direction != p.Direction) continue;
            if (existing.OverlapsWithSameDirection(p))
                return true;
        }
        return false;
    }

    // ==========================================================
    // 3.3 气口检查（核心规则）
    // ==========================================================
    public bool FailBreath(ICanvas canvas, Placement p, CrossType crossType)
    {
        var delta = p.Direction.ToRowColDelta();
        var head = RowCol.At(p.Row, p.Col);

        // ① 首尾气口
        var prev = head - delta;
        var next = head + p.Length * delta;

        if (prev.InBounds(canvas.CanvasSize) && canvas.HasValue(prev)) return true;
        if (next.InBounds(canvas.CanvasSize) && canvas.HasValue(next)) return true;

        // ② 中间格子的两侧气口
        var schema = ExpressionSchemaFactory.Create(p.Length);
        var perp = p.Direction.Perpendicular().ToRowColDelta();

        foreach (var (pos, cellType) in EnumerateCellsWithType(head, delta, schema))
        {
            if (IsCrossPoint(cellType, crossType))
                continue; // 交叉点例外

            var left = pos - perp;
            var right = pos + perp;

            if (left.InBounds(canvas.CanvasSize) && canvas.HasValue(left)) return true;
            if (right.InBounds(canvas.CanvasSize) && canvas.HasValue(right)) return true;
        }

        return false;
    }

    private IEnumerable<(RowCol pos, CellType type)> EnumerateCellsWithType(
        RowCol head, RowCol delta, IExpressionSchema schema)
    {
        for (int i = 0; i < schema.Length; i++)
        {
            yield return (head + i * delta, schema.GetCellType(i));
        }
    }

    private static bool IsCrossPoint(CellType cellType, CrossType crossType)
    {
        return crossType switch
        {
            CrossType.Operator => cellType == CellType.Operator,
            CrossType.Number   => cellType == CellType.Number,
            _ => false
        };
    }

    // ==========================================================
    // 3.4 格子类型检查（避免数字格覆盖 '=' 或符号格）
    // ==========================================================
    public bool FailCellType(Placement p)
    {
        /* 检查一下情况。格子 RowCol(1， 4）是非法的
              0   1   2   3   4   5   6
          ---------------------------------
            0 |      □       □
            1 |  □   ◇   □   =   □
            2 |      □       □
            3 |  □   ◇   □   ◇   □   =   □
            4 |      □       □
            5 |      =       =
            6 |      □       □
          ---------------------------------

        */
        var head = RowCol.At(p.Row, p.Col);
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

    // ==========================================================
    // 3.5 新规则（你未来要启用）
    // ==========================================================
    public bool FailAround(Placement p)
    {
        /* 新的规则，这个的好好想想 （6，4）
               0   1   2   3   4   5   6   7   8   9
          ---------------------------------------------
            0 |          □   ◇   □   ◇   □   =   □
            1 |      □           ◇       ◇       ◇
            2 |  □   ◇   □   =   □       □       □
            3 |      □           ◇       ◇       ◇
            4 |  □   ◇   □   ◇   □   =   □       □
            5 |      □           =       =       =
            6 |      =           □       □       □
            7 |      □   ◇   □   ◇   □   =   □
            8 |              ◇
            9 |              □   ◇   □   ◇   □   =   □
            10 |              =
            11 |              □   ◇   □   ◇   □   =   □
          ---------------------------------------------
          规则总结：
          1. 算式不能过界面
          2. 算式不能与同方向的算式有重合
          3. 算式存在呼吸点（这个点是否稳定，还需在考虑）
          4. 格子类型要对应

          新的规则：
          5. 不同方向的对比
          此算式的周围，不能存在不同方向的首位点
        */
        /*
        1. 此放置点的周围点（不是首尾）
        2. 不同方向的的首位点
        3. 查看命中情况
         */
        var head = RowCol.At(p.Row, p.Col);
        var delta = p.Direction.ToRowColDelta();
        var perp = p.Direction.Perpendicular().ToRowColDelta();

        var around = new HashSet<RowCol>();

        for (int i = 0; i < p.Length; i++)
        {
            var pos = head + i * delta;
            around.Add(pos - perp);
            around.Add(pos + perp);
        }

        foreach (var layout in _layouts)
        {
            if (layout.Direction != p.Direction.Perpendicular())
                continue;

            if (around.Contains(layout.Cells.First())) return true;
            if (around.Contains(layout.Cells.Last())) return true;
        }

        return false;
    }
}

//
// public class PlacementGenerator : IPlacementGenerator
// {
//     private List<ExpressionLayout> _expressionLayouts;
//     private HashSet<Placement> filledPlacemount;
//     private Dictionary<RowCol, CellType> posToCellTypeMap;
//     
//     // === 1. 总入口：Generate ===
//     public IEnumerable<Placement> Generate(
//         ICanvas canvas,
//         int placeLength,
//         CrossType crossType)
//     {
//         Init(canvas);
//
//         foreach (var mount in _expressionLayouts)
//         {
//             var raw = GenerateRaw(mount, placeLength, crossType);
//
//             // 2) 过滤（根据 Canvas + CrossType）
//             var filtered = Filter(canvas, raw, crossType);
//
//             foreach (var placemount in filtered)
//             {
//                 yield return placemount;
//             }
//         }
//     }
//     
//     public IEnumerable<Placement> GenerateCore(
//         ICanvas canvas,
//         int placeLength,
//         CrossType crossType,
//         ExpressionLayout mount)
//     {
//         // 1) 得到可行的原始 Placement（只根据 mount）
//         var raw = GenerateRaw(mount, placeLength, crossType);
//
//         // 2) 过滤（根据 Canvas + CrossType）
//         var filtered = Filter(canvas, raw, crossType);
//
//         return filtered;
//     }
//
//
//     public void Init(ICanvas canvas)
//     {
//         _expressionLayouts = ExpressionLayoutBuilderCore.ExtractLayouts(canvas.CanvasSize, canvas.HasValue, [5, 7]);
//         filledPlacemount = _expressionLayouts
//             .Select(layout => new Placement(layout.Cells.First().Row, layout.Cells.First().Col, layout.Direction, layout.Length))
//             .ToHashSet();
//         posToCellTypeMap = ExpressionLayoutGraphUtils.BuildPosToCellTypeMap(_expressionLayouts);
//     }
//
//     // === 1.1 原始 Placement：不依赖 Canvas，只依赖 mount ===
//     private IEnumerable<Placement> GenerateRaw(ExpressionLayout mount, int placeLength, CrossType crossType)
//     {
//         var offsets = buildOffsets(placeLength, mount, crossType);
//         var startPos = mount.Cells.First();
//         var placeDireaction = mount.Direction.Perpendicular();
//         foreach (var offset in offsets)
//         {
//             var realPos = startPos + offset;
//             yield return new Placement(realPos.Row, realPos.Col, placeDireaction, placeLength);
//         }
//     }
//
//     private IEnumerable<RowCol> buildOffsets(int placeLength, ExpressionLayout mount, CrossType crossType)
//     {
//         var mountSchema = ExpressionSchemaFactory.Create(mount.Length);
//         var placeSchema = ExpressionSchemaFactory.Create(placeLength);
//         if (crossType == CrossType.Operator)
//         {
//             var mountDelta = mount.Direction.ToRowColDelta();
//             var mountPosGroup = Enumerable
//                 .Range(0, mountSchema.Length)
//                 .Where(i => mountSchema.GetCellType(i) == CellType.Operator)
//                 .Select(i => i * mountDelta)
//                 .ToList();
//             
//             var placeDelta = mount.Direction.Perpendicular().ToRowColDelta();
//             var placeDeltaPosGroup = Enumerable
//                 .Range(0, placeSchema.Length)
//                 .Where(i => placeSchema.GetCellType(i) == CellType.Operator)
//                 .Select(i => -i * placeDelta);
//             
//             foreach (var placeDeltaPos in placeDeltaPosGroup)
//             foreach (var mountDeltaPos in mountPosGroup)
//             {
//                 yield return placeDeltaPos + mountDeltaPos;
//             }
//         }
//         else if (crossType == CrossType.Number)
//         {
//             var mountDelta = mount.Direction.ToRowColDelta();
//             var mountPosGroup = Enumerable
//                 .Range(0, mountSchema.Length)
//                 .Where(i => mountSchema.GetCellType(i) == CellType.Number)
//                 .Select(i => i * mountDelta)
//                 .ToList();
//             
//             var placeDelta = mount.Direction.Perpendicular().ToRowColDelta();
//             var placeDeltaPosGroup = Enumerable
//                 .Range(0, placeSchema.Length)
//                 .Where(i => placeSchema.GetCellType(i) == CellType.Number)
//                 .Select(i => -i * placeDelta);
//             
//             foreach (var placeDeltaPos in placeDeltaPosGroup)
//             foreach (var mountDeltaPos in mountPosGroup)
//             {
//                 yield return placeDeltaPos + mountDeltaPos;
//             }
//         }
//     }
//
//     // === 2. 过滤层：Filter ===
//     public IEnumerable<Placement> Filter(
//         ICanvas canvas,
//         IEnumerable<Placement> placements,
//         CrossType crossType)
//     {
//         foreach (var placement in placements)
//         {
//             if (CheckAll(canvas, placement, crossType))
//                 yield return placement;
//         }
//     }
//
//     // === 3. 统一 Check ===
//     public bool CheckAll(ICanvas canvas, Placement p, CrossType crossType)
//     {
//         if (FailBound(canvas, p)) return false;
//         if (FailOverlap(canvas, p)) return false;
//         if (FailBreath(canvas, p, crossType)) return false;
//         if (FailCellType(canvas, p)) return false;
//         // if (FailAround(p)) return false;
//         return true;
//     }
//     // === 3.1 越界检查 ===
//     public bool FailBound(ICanvas canvas, Placement placement)
//     {
//         var delta = placement.Direction.ToRowColDelta();
//
//         var headPos = RowCol.At(placement.Row, placement.Col);
//         if (!headPos.InBounds(canvas.CanvasSize)) return true;
//         
//         var tailPos = headPos + (placement.Length - 1) * placement.Direction.ToRowColDelta();
//         if (!tailPos.InBounds(canvas.CanvasSize)) return true;
//       
//         return false;
//     }
//
//     // === 3.2 重合检查：已有格子不能重合 ===
//     public bool FailOverlap(ICanvas canvas, Placement p)
//     {
//         foreach (var placement in filledPlacemount)
//         {
//             if (IsConflict(placement, p))
//                 return true;
//         }
//         return false;
//     }
//
//     // === 3.3 气口检查（CrossType 决定规则）===
//     public bool FailBreath(ICanvas canvas, Placement p, CrossType crossType)
//     {
//         /*
//          *1. 前后的点
//          * 2. 非交叉点
//          */
//         var placeDelta = p.Direction.ToRowColDelta();
//         var headPos = RowCol.At(p.Row, p.Col);
//         
//         var headPrevPos = headPos + (-1) * placeDelta;
//         if  (headPrevPos.InBounds(canvas.CanvasSize) && canvas.HasValue(headPrevPos)) return true;
//         
//         // var tailNextPos = headPrevPos + p.Length * placeDelta; 
//         var tailNextPos = headPos + p.Length * placeDelta; // 起始点开始
//         if  (tailNextPos.InBounds(canvas.CanvasSize) && canvas.HasValue(tailNextPos)) return true;
//
//         // var headPos = RowCol.At(p.Row, p.Col);
//         var placeSchema = ExpressionSchemaFactory.Create(p.Length);
//         List<RowCol> otherBreathPos = new List<RowCol>();
//         // 其他气口
//         var breathmountPosList = Enumerable
//             .Range(0, placeSchema.Length)
//             .Where(i => 
//                 crossType == CrossType.Operator
//                     ? placeSchema.GetCellType(i) == CellType.Number
//                     :  placeSchema.GetCellType(i) != CellType.Number) // 交叉点为符号格的气口挂点在数字格
//             .Select(i => i * placeDelta + headPos);
//
//         var perpendicularDelta = p.Direction.Perpendicular().ToRowColDelta(); // 正交方向delta
//         foreach (var offset in breathmountPosList)
//         {
//             otherBreathPos.Add(offset + (-1) * perpendicularDelta);
//             otherBreathPos.Add(offset + (1) * perpendicularDelta);
//         }
//
//         foreach (var breathPos in otherBreathPos)
//         {
//             if (breathPos.InBounds(canvas.CanvasSize) && canvas.HasValue(breathPos))
//                 return true;
//         }
//
//         return false;
//     }
//
//     public bool FailCellType(ICanvas canvas, Placement placement)
//     {
//         /* 检查一下情况。格子 RowCol(1， 4）是非法的
//                0   1   2   3   4   5   6 
//            ---------------------------------
//              0 |      □       □             
//              1 |  □   ◇   □   =   □         
//              2 |      □       □             
//              3 |  □   ◇   □   ◇   □   =   □ 
//              4 |      □       □             
//              5 |      =       =             
//              6 |      □       □             
//            ---------------------------------
//            
//          */
//         var startPos = RowCol.At(placement.Row, placement.Col);
//         var placeDelta = placement.Direction.ToRowColDelta();
//         var schema = ExpressionSchemaFactory.Create(placement.Length);
//
//         foreach (var i in Enumerable.Range(0, placement.Length))
//         {
//             var placePos = startPos + i * placeDelta;
//             if (!posToCellTypeMap.TryGetValue(placePos, out var filledCellType))
//                 continue;
//             
//             var placeCellType = schema.GetCellType(i);
//             if (placeCellType != filledCellType)
//             {
//                 return true;
//             }
//         }
//         return false;
//     }
//
//     public bool FailAround(Placement placement)
//     {
//         /* 新的规则，这个的好好想想 （6，4）
//                 0   1   2   3   4   5   6   7   8   9 
//            ---------------------------------------------
//              0 |          □   ◇   □   ◇   □   =   □     
//              1 |      □           ◇       ◇       ◇     
//              2 |  □   ◇   □   =   □       □       □     
//              3 |      □           ◇       ◇       ◇     
//              4 |  □   ◇   □   ◇   □   =   □       □     
//              5 |      □           =       =       =     
//              6 |      =           □       □       □     
//              7 |      □   ◇   □   ◇   □   =   □         
//              8 |              ◇                         
//              9 |              □   ◇   □   ◇   □   =   □ 
//              10 |              =                         
//              11 |              □   ◇   □   ◇   □   =   □ 
//            ---------------------------------------------
//            规则总结：
//            1. 算式不能过界面
//            2. 算式不能与同方向的算式有重合
//            3. 算式存在呼吸点（这个点是否稳定，还需在考虑）
//            4. 格子类型要对应 
//            
//            新的规则：
//            5. 不同方向的对比
//            此算式的周围，不能存在不同方向的首位点
//          */
//         /*
//         1. 此放置点的周围点（不是首尾）
//         2. 不同方向的的首位点
//         3. 查看命中情况
//          */
//         var headPos = RowCol.At(placement.Row, placement.Col);
//         var placeDelta =  placement.Direction.ToRowColDelta();
//
//         var perpendicularDir = placement.Direction.Perpendicular();
//         var perpendicularDelta = perpendicularDir.ToRowColDelta();
//         
//         var aroundPosSet = new HashSet<RowCol>();
//         for (var i = 0; i < placement.Length; i++)
//         {
//             var placePos = headPos + i * placeDelta;
//             aroundPosSet.Add(placePos + (-1) * perpendicularDelta); //上
//             aroundPosSet.Add(placePos + (1) * perpendicularDelta);  //下 两个点
//         }
//         
//         // 正交方向放置点的其实点
//         foreach (var layout in _expressionLayouts)
//         {
//             if (layout.Direction != perpendicularDir) continue;
//             
//             var startPos = layout.Cells.First();
//             if (aroundPosSet.Contains(startPos)) return true;
//             
//             var endPos = layout.Cells.Last();
//             if (aroundPosSet.Contains(endPos)) return true;
//         }
//         
//         return false;
//     }
//     
//     public static bool IsConflict(Placement a, Placement b)
//     {
//         // 1. 方向必须一致
//         if (a.Direction != b.Direction)
//             return false;
//
//         var delta = a.Direction.ToRowColDelta();
//
//         // 2. 计算两个 placement 的端点
//         var a1 = RowCol.At(a.Row, a.Col);
//         var a2 = a1 + (a.Length - 1) * delta;
//
//         var b1 =  RowCol.At(b.Row, b.Col);
//         var b2 = b1 + (b.Length - 1) * delta;
//
//         // 3. 水平方向检查
//         if (delta.Col != 0)  // Horizontal
//         {
//             // 不在同一行 => 不相交
//             if (a1.Row != b1.Row)
//                 return false;
//
//             int aMin = Math.Min(a1.Col, a2.Col);
//             int aMax = Math.Max(a1.Col, a2.Col);
//             int bMin = Math.Min(b1.Col, b2.Col);
//             int bMax = Math.Max(b1.Col, b2.Col);
//
//             return !(aMax < bMin || bMax < aMin);
//         }
//
//         // 4. 垂直方向检查
//         if (delta.Row != 0) // Vertical
//         {
//             // 不在同一列 => 不相交
//             if (a1.Col != b1.Col)
//                 return false;
//
//             int aMin = Math.Min(a1.Row, a2.Row);
//             int aMax = Math.Max(a1.Row, a2.Row);
//             int bMin = Math.Min(b1.Row, b2.Row);
//             int bMax = Math.Max(b1.Row, b2.Row);
//
//             return !(aMax < bMin || bMax < aMin);
//         }
//
//         throw new NotSupportedException("Unknown direction");
//     }
//
//
// }
