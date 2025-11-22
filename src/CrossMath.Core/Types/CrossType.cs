namespace CrossMath.Core.Types;

public enum CrossType
{
    None,       // 没有相交
    Number,     // 数字与数字
    Operator,   // 运算符与运算符
    Equal,      // 等号与等号
    Invalid     // 非法相交方式 数字x运算符 运算符x等号 等号x数字
}