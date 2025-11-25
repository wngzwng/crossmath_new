using System.Diagnostics;
using CrossMath.Core.Expressions.Core;
using CrossMath.Core.Types;
using CrossMath.Core.Utils;

namespace CrossMath.Core.ExpressionSolvers.Expression7Solvers;

public class Expression7Solver: IExpressionSolver
{
    public int SupportedLength => 7;
    public IEnumerable<IExpression> Solve(IExpression expression, ExpressionSolveContext context)
    {
        var exp7 = AsExpression7(expression);
        return CoreSolve(exp7, context);
    }

    private static Expression7 AsExpression7(IExpression expression)
    {
        if (expression is not Expression7 exp7)
            throw new ArgumentException("Only Expression7 is supported by this solver.", nameof(expression));
        return exp7;
    }
    
    
    private IEnumerable<Expression7> CoreSolve(Expression7 template, ExpressionSolveContext context)
    {
        foreach (var withOperator in EnumerateWithFilledOperator(template, context))
        {
            Debug.Assert(!withOperator.HasEmptySymbol());
            var op1 = withOperator.Op1!.Value;
            var op2 = withOperator.Op2!.Value;

            var availableNumbers = context.NumPool.UniqueNumbers;

            foreach (var (a, b, c, t) in FindValidNumberQuads(availableNumbers, op1, op2,
                         knownA: template.A, knownB: template.B, knownC: template.C, knownT: template.T))
            {
                var solved = CreateSolvedExpression(a, b, c, t, op1, op2);

                bool isValid = context.Validator == null || context.Validator.Validate(template, solved, context);
                if (isValid)
                    yield return solved;
            }
        }
    }

    private IEnumerable<Expression7> EnumerateWithFilledOperator(Expression7 template, ExpressionSolveContext context)
    {
        if (!template.HasEmptySymbol())
        {
            yield return template;
            yield break;
        }

        var ops = context.OpPool.UniqueOperators.ToList();

        IEnumerable<OpType> op1Source = template.Op1.HasValue ? new[] { template.Op1!.Value } : ops;
        IEnumerable<OpType> op2Source = template.Op2.HasValue ? new[] { template.Op2!.Value } : ops;
        
        foreach (var op1 in op1Source)
        foreach (var op2 in op2Source)
        {
            yield return template.WithOperator(op1, op2);
        }


    }
    
    private IEnumerable<(int a, int b, int c, int t)> FindValidNumberQuads(
        IEnumerable<int> availableNumbers,
        OpType op1,
        OpType op2,
        int? knownA,
        int? knownB,
        int? knownC,
        int? knownT)
    {
        var nums = availableNumbers.Distinct().ToList();

        // 根据 knownX 决定候选列表
        IEnumerable<int> A = knownA is int a0 ? new[] { a0 } : nums;
        IEnumerable<int> B = knownB is int b0 ? new[] { b0 } : nums;
        IEnumerable<int> C = knownC is int c0 ? new[] { c0 } : nums;

        foreach (int a in A)
        foreach (int b in B)
        foreach (int c in C)
        {
            // 使用 RPN / double / 严格整数检查求值
            if (!TryForwardCompute(a, b, c, op1, op2, out int t))
                continue;

            // 如果 T 已知，必须匹配
            if (knownT.HasValue && t != knownT.Value)
                continue;

            yield return (a, b, c, t);
        }
    }
    
    private static bool TryForwardCompute(int a, int b, int c, OpType op1, OpType op2, out int result)
    {
        result = 0;

        // 1. 转换为 RPN
        var nums = new double[] { a, b, c };
        var ops = new OpType[] { op1, op2 };

        var rpn = ExpressionEvalUtils.ToRpn(nums, ops);

        // 2. 求浮点值
        if (!ExpressionEvalUtils.TryComputeRpnDouble(rpn, out var value))
            return false;

        // 3. 必须是整数
        if (!ExpressionEvalUtils.IsInteger(value))
            return false;

        // 4. 输出最终整数（floor/round 均可，整数差别无影响）
        result = (int)Math.Round(value);
        return true;
    }
    
    
    private static Expression7 CreateSolvedExpression(int a, int b, int c, int t, OpType op1,  OpType op2)
    {
        var exp = (Expression7)ExpressionFactory.CreateEmpty(7);
        exp.A = a;
        exp.B = b;
        exp.C = c;
        exp.T = t;
        
        exp.Op1 = op1;
        exp.Op2 = op2;
        
        return exp;
    }
    


}