using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Expressions.Schema;
using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Types;

namespace CrossMath.Core.Generators.PlacementGenerators;

public class PlacementGenerator : IPlacementGenerator
{
    private List<ExpressionLayout> _expressionLayouts;
    private HashSet<Placement> filledPlacemount;
    private Dictionary<RowCol, CellType> posToCellTypeMap;
    
    // === 1. 总入口：Generate ===
    public IEnumerable<Placement> Generate(
        ICanvas canvas,
        int placeLength,
        CrossType crossType)
    {
        Init(canvas);

        foreach (var mount in _expressionLayouts)
        {
            var raw = GenerateRaw(mount, placeLength, crossType);

            // 2) 过滤（根据 Canvas + CrossType）
            var filtered = Filter(canvas, raw, crossType);

            foreach (var placemount in filtered)
            {
                yield return placemount;
            }
        }
    }
    
    public IEnumerable<Placement> GenerateCore(
        ICanvas canvas,
        int placeLength,
        CrossType crossType,
        ExpressionLayout mount)
    {
        // 1) 得到可行的原始 Placement（只根据 mount）
        var raw = GenerateRaw(mount, placeLength, crossType);

        // 2) 过滤（根据 Canvas + CrossType）
        var filtered = Filter(canvas, raw, crossType);

        return filtered;
    }


    public void Init(ICanvas canvas)
    {
        _expressionLayouts = ExpressionLayoutBuilderCore.ExtractLayouts(canvas.CanvasSize, canvas.HasValue, [5, 7]);
        filledPlacemount = _expressionLayouts
            .Select(layout => new Placement(layout.Cells.First().Row, layout.Cells.First().Row, layout.Direction, layout.Length))
            .ToHashSet();
        posToCellTypeMap = ExpressionLayoutGraphUtils.BuildPosToCellTypeMap(_expressionLayouts);
    }

    // === 1.1 原始 Placement：不依赖 Canvas，只依赖 mount ===
    private IEnumerable<Placement> GenerateRaw(ExpressionLayout mount, int placeLength, CrossType crossType)
    {
        var offsets = buildOffsets(placeLength, mount, crossType);
        var startPos = mount.Cells.First();
        var placeDireaction = mount.Direction.Perpendicular();
        foreach (var offset in offsets)
        {
            var realPos = startPos + offset;
            yield return new Placement(realPos.Row, realPos.Col, placeDireaction, placeLength);
        }
    }

    private IEnumerable<RowCol> buildOffsets(int placeLength, ExpressionLayout mount, CrossType crossType)
    {
        var mountSchema = ExpressionSchemaFactory.Create(mount.Length);
        var placeSchema = ExpressionSchemaFactory.Create(placeLength);
        if (crossType == CrossType.Operator)
        {
            var mountDelta = mount.Direction.ToRowColDelta();
            var mountPosGroup = Enumerable
                .Range(0, mountSchema.Length)
                .Where(i => mountSchema.GetCellType(i) == CellType.Operator)
                .Select(i => i * mountDelta)
                .ToList();
            
            var placeDelta = mount.Direction.Perpendicular().ToRowColDelta();
            var placeDeltaPosGroup = Enumerable
                .Range(0, placeSchema.Length)
                .Where(i => placeSchema.GetCellType(i) == CellType.Operator)
                .Select(i => -i * placeDelta);
            
            foreach (var placeDeltaPos in placeDeltaPosGroup)
            foreach (var mountDeltaPos in mountPosGroup)
            {
                yield return placeDeltaPos + mountDeltaPos;
            }
        }
        else if (crossType == CrossType.Number)
        {
            var mountDelta = mount.Direction.ToRowColDelta();
            var mountPosGroup = Enumerable
                .Range(0, mountSchema.Length)
                .Where(i => mountSchema.GetCellType(i) == CellType.Number)
                .Select(i => i * mountDelta)
                .ToList();
            
            var placeDelta = mount.Direction.Perpendicular().ToRowColDelta();
            var placeDeltaPosGroup = Enumerable
                .Range(0, placeSchema.Length)
                .Where(i => placeSchema.GetCellType(i) == CellType.Number)
                .Select(i => -i * placeDelta);
            
            foreach (var placeDeltaPos in placeDeltaPosGroup)
            foreach (var mountDeltaPos in mountPosGroup)
            {
                yield return placeDeltaPos + mountDeltaPos;
            }
        }
    }

    // === 2. 过滤层：Filter ===
    public IEnumerable<Placement> Filter(
        ICanvas canvas,
        IEnumerable<Placement> placements,
        CrossType crossType)
    {
        foreach (var placement in placements)
        {
            if (CheckAll(canvas, placement, crossType))
                yield return placement;
        }
    }

    // === 3. 统一 Check ===
    public bool CheckAll(ICanvas canvas, Placement p, CrossType crossType)
    {
        if (FailBound(canvas, p)) return false;
        if (FailOverlap(canvas, p)) return false;
        if (FailBreath(canvas, p, crossType)) return false;
        if (FailCellType(canvas, p)) return false;
        return true;
    }
    // === 3.1 越界检查 ===
    public bool FailBound(ICanvas canvas, Placement placement)
    {
        var delta = placement.Direction.ToRowColDelta();

        var headPos = RowCol.At(placement.Row, placement.Col);
        if (!headPos.InBounds(canvas.CanvasSize)) return true;
        
        var tailPos = headPos + (placement.Length - 1) * placement.Direction.ToRowColDelta();
        if (!tailPos.InBounds(canvas.CanvasSize)) return true;
      
        return false;
    }

    // === 3.2 重合检查：已有格子不能重合 ===
    public bool FailOverlap(ICanvas canvas, Placement p)
    {
        foreach (var placement in filledPlacemount)
        {
            if (IsConflict(placement, p))
                return true;
        }
        return false;
    }

    // === 3.3 气口检查（CrossType 决定规则）===
    public bool FailBreath(ICanvas canvas, Placement p, CrossType crossType)
    {
        /*
         *1. 前后的点
         * 2. 非交叉点
         */
        var placeDelta = p.Direction.ToRowColDelta();
        var headPrevPos = RowCol.At(p.Row, p.Col) + (-1) * placeDelta;
        if  (headPrevPos.InBounds(canvas.CanvasSize) && canvas.HasValue(headPrevPos)) return true;
        
        var tailNextPos = headPrevPos + p.Length * placeDelta;
        if  (tailNextPos.InBounds(canvas.CanvasSize) && canvas.HasValue(tailNextPos)) return true;

        var headPos = RowCol.At(p.Row, p.Col);
        var placeSchema = ExpressionSchemaFactory.Create(p.Length);
        List<RowCol> otherBreathPos = new List<RowCol>();
        // 其他气口
        var breathmountPosList = Enumerable
            .Range(0, placeSchema.Length)
            .Where(i => 
                crossType == CrossType.Operator
                    ? placeSchema.GetCellType(i) == CellType.Number
                    :  placeSchema.GetCellType(i) != CellType.Number) // 交叉点为符号格的气口挂点在数字格
            .Select(i => i * placeDelta + headPos);

        var perpendicularDelta = p.Direction.Perpendicular().ToRowColDelta(); // 正交方向delta
        foreach (var offset in breathmountPosList)
        {
            otherBreathPos.Add(offset + (-1) * perpendicularDelta);
            otherBreathPos.Add(offset + (1) * perpendicularDelta);
        }

        foreach (var breathPos in otherBreathPos)
        {
            if (breathPos.InBounds(canvas.CanvasSize) && canvas.HasValue(breathPos))
                return true;
        }

        return false;
    }

    public bool FailCellType(ICanvas canvas, Placement placement)
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
        var startPos = RowCol.At(placement.Row, placement.Col);
        var placeDelta = placement.Direction.ToRowColDelta();
        var schema = ExpressionSchemaFactory.Create(placement.Length);

        foreach (var i in Enumerable.Range(0, placement.Length))
        {
            var placePos = startPos + i * placeDelta;
            if (!posToCellTypeMap.TryGetValue(placePos, out var filledCellType))
                continue;
            
            var placeCellType = schema.GetCellType(i);
            if (placeCellType != filledCellType)
            {
                return true;
            }
        }
        return false;
    }
    
    public static bool IsConflict(Placement a, Placement b)
    {
        // 1. 方向必须一致
        if (a.Direction != b.Direction)
            return false;

        var delta = a.Direction.ToRowColDelta();

        // 2. 计算两个 placement 的端点
        var a1 = RowCol.At(a.Row, a.Col);
        var a2 = a1 + (a.Length - 1) * delta;

        var b1 =  RowCol.At(b.Row, b.Col);
        var b2 = b1 + (b.Length - 1) * delta;

        // 3. 水平方向检查
        if (delta.Col != 0)  // Horizontal
        {
            // 不在同一行 => 不相交
            if (a1.Row != b1.Row)
                return false;

            int aMin = Math.Min(a1.Col, a2.Col);
            int aMax = Math.Max(a1.Col, a2.Col);
            int bMin = Math.Min(b1.Col, b2.Col);
            int bMax = Math.Max(b1.Col, b2.Col);

            return !(aMax < bMin || bMax < aMin);
        }

        // 4. 垂直方向检查
        if (delta.Row != 0) // Vertical
        {
            // 不在同一列 => 不相交
            if (a1.Col != b1.Col)
                return false;

            int aMin = Math.Min(a1.Row, a2.Row);
            int aMax = Math.Max(a1.Row, a2.Row);
            int bMin = Math.Min(b1.Row, b2.Row);
            int bMax = Math.Max(b1.Row, b2.Row);

            return !(aMax < bMin || bMax < aMin);
        }

        throw new NotSupportedException("Unknown direction");
    }


}
