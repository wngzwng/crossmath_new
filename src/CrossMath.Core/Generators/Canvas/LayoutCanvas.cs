using CrossMath.Core.Types;

namespace CrossMath.Core.Generators.Canvas;

public class LayoutCanvas:ICanvas
{
    public Size CanvasSize { get; }

    private int[] grid;
    
    public LayoutCanvas(int width, int height)
    {
        
        CanvasSize = new Size(width, height);
        grid = new int[width * height]; // all zero by default
    }

    public LayoutCanvas(Size size)
    {
       CanvasSize = size;
       grid = new int[size.Width * size.Height];
    }

    public bool HasValue(RowCol pos)
    {
        if (!pos.InBounds(CanvasSize))
        {
            return false;
        }

        return grid[pos.Row * CanvasSize.Width + pos.Col] != 0;
    }

    public object GetValue(RowCol pos)
    {
        if (!pos.InBounds(CanvasSize))
        {
            return null;
        }
        return grid[pos.Row * CanvasSize.Width + pos.Col];
    }

    public void Clear(RowCol pos)
    {
        if (!pos.InBounds(CanvasSize))
        {
            return;
        }
        
        grid[pos.Row * CanvasSize.Width + pos.Col] = 0;
    }

    public bool SetValue(RowCol pos, int? value)
    {
        if (!pos.InBounds(CanvasSize))
        {
            return false;
        }

        grid[pos.Row * CanvasSize.Width + pos.Col] = value.HasValue ? value.Value : 1;
        return true;
    }

    public ICanvas Clone()
    {
        return new LayoutCanvas(CanvasSize)
        {
            grid = this.grid.ToArray()
        };
    }
}