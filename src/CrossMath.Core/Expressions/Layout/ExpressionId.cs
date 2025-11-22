namespace CrossMath.Core.Expressions.Layout;

public readonly record struct ExpressionId(string Value) 
{ 
    private static int _counter = 0; 
    public static ExpressionId New() => new($"E{Interlocked.Increment(ref _counter):D3}"); 
    public static ExpressionId Parse(string s) => new(s); 
    public override string ToString() => Value; 
    public static implicit operator string(ExpressionId id) => id.Value; 
}
