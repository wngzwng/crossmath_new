using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using CrossMath.Core.Expressions.Core;
using CrossMath.Core.Types;

namespace CrossMath.Core.ExpressionSolvers.Expression5Solvers;

/// <summary>
/// Solver for 5-character expressions (e.g., "a+b=c", "a*b=c").
/// Designed for high performance and strong pruning when partial values are known.
/// </summary>
public partial class Expression5Solver : IExpressionSolver
{
    public IEnumerable<IExpression> Solve(IExpression expression, ExpressionSolveContext context)
    {
        var exp5 = AsExpression5(expression);
        return CoreSolve(exp5, context);
    }

    private static Expression5 AsExpression5(IExpression expression)
    {
        if (expression is not Expression5 exp5)
            throw new ArgumentException("Only Expression5 is supported by this solver.", nameof(expression));
        return exp5;
    }

    private IEnumerable<Expression5> CoreSolve(Expression5 template, ExpressionSolveContext context)
    {
        foreach (var withOperator in EnumerateWithFilledOperator(template, context))
        {
            Debug.Assert(withOperator.Op.HasValue);
            var op = withOperator.Op!.Value;

            var availableNumbers = context.NumPool.UniqueNumbers;

            foreach (var (a, b, c) in FindValidNumberTriples(availableNumbers, op,
                       knownA: template.A, knownB: template.B, knownC: template.C))
            {
                var solved = CreateSolvedExpression(a, b, c, op);

                bool isValid = context.Validator == null || context.Validator.Validate(template, solved, context);
                if (isValid)
                    yield return solved;
            }
        }
    }

    /// <summary>
    /// Fills the operator if missing, or yields the original when already present.
    /// When at least two operands are known, aggressively prunes impossible operators.
    /// </summary>
    private IEnumerable<Expression5> EnumerateWithFilledOperator(Expression5 template, ExpressionSolveContext context)
    {
        if (template.Op.HasValue)
        {
            yield return template;
            yield break;
        }

        int knownOperandCount = CountKnownOperands(template);

        if (knownOperandCount < 2)
        {
            // No pruning possible – enumerate all allowed operators
            foreach (var op in context.OpPool.UniqueOperators)
                yield return template.WithOperator(op);
        }
        else
        {
            // Strong pruning: only operators that are mathematically possible survive
            foreach (var op in InferPossibleOperators(template, context))
                yield return template.WithOperator(op);
        }
    }

    private static int CountKnownOperands(Expression5 exp)
        => (exp.A.HasValue ? 1 : 0) + (exp.B.HasValue ? 1 : 0) + (exp.C.HasValue ? 1 : 0);

    /// <summary>
    /// When ≥2 operands are known, infer which operators could possibly satisfy the equation.
    /// This single method often gives 10–100× performance improvement.
    /// </summary>
    private static IEnumerable<OpType> InferPossibleOperators(Expression5 exp, ExpressionSolveContext context)
    {
        int? a = exp.A, b = exp.B, c = exp.C;
        foreach (var op in context.OpPool.UniqueOperators)
        {
            // All three known → strict validation
            if (a.HasValue && b.HasValue && c.HasValue)
            {
                if (CheckExactEquation(a.Value, b.Value, c.Value, op))
                    yield return op;
                continue;
            }

            // Exactly one missing → reverse-engineer the missing value and see if it's feasible
            bool possible =
                (!a.HasValue && b.HasValue && c.HasValue && TrySolveForA(b.Value, c.Value, op, out _)) ||
                (a.HasValue && !b.HasValue && c.HasValue && TrySolveForB(a.Value, c.Value, op, out _)) ||
                (a.HasValue && b.HasValue && !c.HasValue && TryForwardCompute(a.Value, b.Value, op, out _));

            if (possible)
                yield return op;
        }
    }

    private static bool CheckExactEquation(int a, int b, int c, OpType op) => op switch
    {
        OpType.Add => a + b == c,
        OpType.Sub => a - b == c,
        OpType.Mul => a * b == c,
        OpType.Div => b != 0 && a % b == 0 && a / b == c,
        _ => false
    };

    private static bool TryForwardCompute(int a, int b, OpType op, out int c)
    {
        c = op switch
        {
            OpType.Add => a + b,
            OpType.Sub => a - b,
            OpType.Mul => a * b,
            OpType.Div => b != 0 && a % b == 0 ? a / b : int.MinValue,
            _ => int.MinValue
        };
        return c != int.MinValue;
    }

    private static bool TrySolveForA(int b, int c, OpType op, out int a)
    {
        a = op switch
        {
            OpType.Add => c - b,
            OpType.Sub => c + b,
            OpType.Mul => b != 0 && c % b == 0 ? c / b : int.MinValue,
            OpType.Div => b * c,
            _ => int.MinValue
        };
        return a != int.MinValue;
    }

    private static bool TrySolveForB(int a, int c, OpType op, out int b)
    {
        b = op switch
        {
            OpType.Add => c - a,
            OpType.Sub => a - c,
            OpType.Mul => a != 0 && c % a == 0 ? c / a : int.MinValue,
            OpType.Div => c != 0 && a % c == 0 ? a / c : int.MinValue,
            _ => int.MinValue
        };
        return b != int.MinValue;
    }

    /// <summary>
    /// Enumerates all (a,b,c) triples that satisfy a op b = c,
    /// respecting any known values passed by the caller.
    /// </summary>
    private static IEnumerable<(int a, int b, int c)> FindValidNumberTriples(
        IEnumerable<int> availableNumbers,
        OpType op,
        int? knownA = null,
        int? knownB = null,
        int? knownC = null)
    {
        var distinctNumbers = availableNumbers.Distinct().ToList();

        IEnumerable<int> aCandidates = knownA.HasValue ? new[] { knownA.Value } : distinctNumbers;
        IEnumerable<int> bCandidates = knownB.HasValue ? new[] { knownB.Value } : distinctNumbers;

        foreach (int a in aCandidates)
        foreach (int b in bCandidates)
        {
            if (!TryForwardCompute(a, b, op, out int c))
                continue;

            if (!knownC.HasValue || c == knownC.Value)
                yield return (a, b, c);
        }
    }

    private static Expression5 CreateSolvedExpression(int a, int b, int c, OpType op)
    {
        var exp = (Expression5)ExpressionFactory.CreateEmpty(5);
        exp.A = a;
        exp.B = b;
        exp.C = c;
        exp.Op = op;
        return exp;
    }
}