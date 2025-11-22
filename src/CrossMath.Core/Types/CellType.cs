namespace CrossMath.Core.Types;

public enum CellType: byte
{
    /// <summary> 空盘阶段未定义（没有任何字符） </summary>
    Unspecified = 0,
    
    Number = 1,     // 数字格
    Operator = 2,   // 运算符格 (+ - * /)
    Equal = 3,  // 等号格

    /// <summary> 数据非法，不属于任何合法符号类别 </summary>
    Invalid = 255
}
