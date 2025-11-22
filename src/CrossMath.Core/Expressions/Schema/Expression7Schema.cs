using CrossMath.Core.Types;

namespace CrossMath.Core.Expressions.Schema;

public sealed class Expression7Schema : IExpressionSchema  
{  
    private static readonly CellType[] Roles =  
    {        
        CellType.Number,   // 0: a  
        CellType.Operator, // 1: op1  
        CellType.Number,   // 2: b  
        CellType.Operator, // 3: op2  
        CellType.Number,   // 4: c  
        CellType.Equal, // 5: =  
        CellType.Number    // 6: d  
    };

    public int Length => 7;

    public CellType GetCellType(int index)  
    {        if (index < 0 || index >= Roles.Length)  
            throw new ArgumentOutOfRangeException(nameof(index));  
        return Roles[index];  
    }  
    public IReadOnlyList<CellType> GetAllCellTypes() => Roles;  
}