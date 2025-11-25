using System.Text;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Generators.Canvas;

public static class CanvasExtensions
{
    public static bool TryApplyPlacement(this ICanvas canvas, Placement placement, out List<RowCol>? realSettingList)
    {
        // 1. 点的越界检查
        realSettingList = null;

        var headPos = RowCol.At(placement.Row, placement.Col);
        if (!headPos.InBounds(canvas.CanvasSize)) return false;
        
        var tailPos = headPos + (placement.Length - 1) * placement.Direction.ToRowColDelta();
        if (!tailPos.InBounds(canvas.CanvasSize)) return false;

        realSettingList = new List<RowCol>();
        var deltaRowCol = placement.Direction.ToRowColDelta(); 
        for (var i = 0; i < placement.Length; i++)
        {
            var pos = headPos + i * deltaRowCol;
            if (canvas.HasValue(pos)) continue;

            canvas.SetValue(pos);
            realSettingList.Add(pos);
        }
        return true;
    }

    public static (RowCol minPos, RowCol maxPos) GetMinMaxPosition(this ICanvas canvas)
    {
        var allFilled = canvas.CanvasSize
            .AllPositions()
            .Where(canvas.HasValue)
            .ToList();

        if (allFilled.Count == 0)
            return (RowCol.Zero, RowCol.Zero);   // 或者你定义的 Empty/None

        int minRow = allFilled.Min(p => p.Row);
        int maxRow = allFilled.Max(p => p.Row);
        int minCol = allFilled.Min(p => p.Col);
        int maxCol = allFilled.Max(p => p.Col);

        return (new RowCol(minRow, minCol), new RowCol(maxRow, maxCol));
    }
    
    public static Size GetBoundingBoxSize(RowCol p1, RowCol p2)
    {
        int width  = Math.Abs(p2.Col - p1.Col) + 1;
        int height = Math.Abs(p2.Row - p1.Row) + 1;
        return  new Size(width, height);
    }
    
    public static Size GetBoundingBoxSize(this ICanvas canvas)
    {
        var (minPos, maxPos) = canvas.GetMinMaxPosition();
        return GetBoundingBoxSize(minPos, maxPos);
    }


    public static BoardLayout ExportBoardLayout(this ICanvas canvas, bool needCrop = true)
    {
        if (needCrop)
            return ExportCropped(canvas);

        return ExportFull(canvas);
    }

    private static BoardLayout ExportFull(ICanvas canvas)
    {
        int width = canvas.CanvasSize.Width;
        int height = canvas.CanvasSize.Height;

        var sb = new StringBuilder(width * height);

        for (int r = 0; r < height; r++)
        {
            for (int c = 0; c < width; c++)
            {
                sb.Append(canvas.HasValue(new RowCol(r, c)) ? '1' : '0');
            }
        }

        return new BoardLayout(sb.ToString(), width, height);
    }

    private static BoardLayout ExportCropped(ICanvas canvas)
    {
        var (minPos, maxPos) = canvas.GetMinMaxPosition();

        // 如果画布里没有任何值，则返回空布局（你可以改成抛异常或返回 null）
        if (minPos == RowCol.Zero &&  maxPos == RowCol.Zero)
        {
            return new BoardLayout("", 0, 0);
        }

        var size = GetBoundingBoxSize(minPos, maxPos);
        var sb = new StringBuilder(size.Width * size.Height);

        for (int r = minPos.Row; r <= maxPos.Row; r++)
        {
            for (int c = minPos.Col; c <= maxPos.Col; c++)
            {
                var pos = new RowCol(r, c);
                sb.Append(canvas.HasValue(pos) ? '1' : '0');
            }
        }

        return new BoardLayout(sb.ToString(), size);
    }
    

}