namespace CrossMath.Core.ExpressionSolvers.ExpressionValidators;

public enum ValidationMode
{
    /// <summary>
    /// 1. 所有格子都必须来自池子
    /// </summary>
    FullPoolCheck,
    
    /// <summary>
    /// 2. 只有空位检查，且来自对应池子
    /// </summary>
    BlankOnlyPoolCheck,
    /// <summary>
    /// 3. 只有空位检查，数字和符号都严格消耗
    /// </summary>
    FullDiscreteConsume   
}