namespace CrossMath.Core.ExpressionSolvers.Pipeline;

public class ExpressionSolveOptions
{
    public bool RespectOriginalFixedValues { get; set; } = true;

    public IExpressionValidator Validator { get; set; }
    public IOperatorStrategy OperatorStrategy { get; set; }
    
    public IValueStrategy ValueStrategy { get;}
}
