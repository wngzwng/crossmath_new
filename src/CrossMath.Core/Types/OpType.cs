namespace CrossMath.Core.Types;

public enum OpType : byte
{
     /// <summary>
     /// 未指定 —— 代表“目前还不知道该用什么运算符”，通常用于初始化阶段或模板阶段。
     /// </summary>
     Unspecified = 0,

     /// <summary>
     /// 加法
     /// </summary>
     Add = 1,

     /// <summary>
     /// 减法
     /// </summary>
     Sub = 2,

     /// <summary>
     /// 乘法
     /// </summary>
     Mul = 3,

     /// <summary>
     /// 除法
     /// </summary>
     Div = 4,

     /// <summary>
     /// 非法 —— 代表“这是错误数据 / 不可能出现的符号”
     /// </summary>
     Invalid = 255
}
