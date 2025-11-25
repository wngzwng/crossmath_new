using CrossMath.Core.Types;

namespace CrossMath.Core.Generators.Canvas;

public interface ICanvas
{
    public int Width => CanvasSize.Width;
    public int Height => CanvasSize.Height;
    
    public Size CanvasSize { get; }


    public bool HasValue(RowCol pos);
    
    public bool SetValue(RowCol pos, int? value = null);
    public object GetValue(RowCol pos);
    public void Clear(RowCol pos);
    
    public ICanvas Clone();
}