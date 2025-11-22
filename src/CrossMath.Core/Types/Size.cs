namespace CrossMath.Core.Types;

public readonly record struct Size(int Width, int Height)  
{  
    // 边界检查（与 RowCol 协作）  
    public bool Contains(RowCol position)  
        => position.Row >= 0 && position.Row < Height &&   
           position.Col >= 0 && position.Col < Width;  
    public bool Contains(int row, int col)  
        => row >= 0 && row < Height && col >= 0 && col < Width;  
    // 从 Size 创建边界  
    public RowCol MaxBounds => new(Height - 1, Width - 1);  
    // 所有有效位置的枚举  
    public IEnumerable<RowCol> AllPositions()  
    {        for (int row = 0; row < Height; row++)  
        for (int col = 0; col < Width; col++)  
            yield return new RowCol(row, col);  
    }
}
