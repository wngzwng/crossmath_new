namespace CrossMath.Core.Expressions.Layout;

public readonly record struct ExpressionId(string Value) 
{ 
    private static int _counter = 0; 
    public static ExpressionId New() => new($"E{Interlocked.Increment(ref _counter):D3}"); 
    
    public static ExpressionId New(int id) => new($"E{id:D3}");

    /// <summary>手动生成任意字符串 ID（你自己确保格式正确）</summary>
    public static ExpressionId New(string value) => new(value);
    public static ExpressionId Parse(string s) => new(s); 
    public override string ToString() => Value; 
    public static implicit operator string(ExpressionId id) => id.Value; 
}
