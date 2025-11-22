namespace CrossMath.Core.ExpressionSolvers;

public enum ValidationMode { 
    None, // 完全不验证，随便枚举所有可能 
    Custom, // 用户自己传 Predicate 进来 
    Partial, // 只验证「已填好的空」必须正确（对空位验证） 
    All // 只有完全算对的才返回（对算式全部验证） 
}