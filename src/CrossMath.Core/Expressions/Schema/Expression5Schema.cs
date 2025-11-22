using CrossMath.Core.Types;

namespace CrossMath.Core.Expressions.Schema;

public sealed class Expression5Schema : IExpressionSchema  
{  
    private static readonly CellType[] Roles =  
    {        
        CellType.Number,   // 0: a  
        CellType.Operator, // 1: +  
        CellType.Number,   // 2: b  
        CellType.Equal, // 3: =  
        CellType.Number    // 4: c  
    };  
  
    public int Length => 5;  
  
    public CellType GetCellType(int index)  
    {        if (index < 0 || index >= Roles.Length)  
            throw new ArgumentOutOfRangeException(nameof(index));  
        return Roles[index];  
    }  
    public IReadOnlyList<CellType> GetAllCellTypes() => Roles;  
}