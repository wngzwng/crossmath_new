using CrossMath.Core.Types;

namespace CrossMath.Core.Expressions.Schema;

public interface IExpressionSchema  
{  
    /// <summary>表达式结构长度</summary>  
    int Length { get; }  
  
    /// <summary>根据索引返回格子的逻辑角色（数字格/运算符格）</summary>  
    CellType GetCellType(int index);  
  
    /// <summary>获取所有索引对应的逻辑角色（缓存友好）</summary>  
    IReadOnlyList<CellType> GetAllCellTypes();  
}