using CrossMath.Core.Expressions.Core;
using CrossMath.Core.ExpressionSolvers.ExpressionValidators;
using CrossMath.Core.ExpressionSolvers.NumberPools;
using CrossMath.Core.ExpressionSolvers.OperatorPools;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.ExpressionSolvers;

public sealed class ExpressionSolveContext
{
    public INumberPool NumPool { get; set; }
    public IOperatorPool OpPool { get; set; }
    public IExpressionValidator? Validator { get; set; }
    
    public BoardData? Board { get; set; }
}