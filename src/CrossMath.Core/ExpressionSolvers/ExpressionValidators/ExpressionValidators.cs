using System.ComponentModel.Design;
using CrossMath.Core.Expressions.Core;
using CrossMath.Core.Expressions.Schema;
using CrossMath.Core.ExpressionSolvers.NumberPools;
using CrossMath.Core.ExpressionSolvers.OperatorPools;
using CrossMath.Core.Types;
using CrossMath.Core.Models;

namespace CrossMath.Core.ExpressionSolvers.ExpressionValidators;

public class ExpressionValidator : IExpressionValidator
{
    private readonly ValidationMode _mode;

    public ExpressionValidator(ValidationMode mode = ValidationMode.BlankOnlyPoolCheck)
    {
        _mode = mode;
    }

    public bool Validate(IExpression original, IExpression solvedExp, ExpressionSolveContext context)
    {
        if (original.Length != solvedExp.Length) return false;

        return _mode switch
        {
            ValidationMode.BlankOnlyPoolCheck   => ValidateBlankOnly(original, solvedExp, context),
            ValidationMode.FullPoolCheck        => ValidateFullPool(original, solvedExp, context),
            ValidationMode.FullDiscreteConsume  => ValidateDiscreteConsume(original, solvedExp, context),
            _ => throw new NotSupportedException($"Unknown mode: {_mode}")
        };
    }

    // 模式1：只检查空白格（最常见）
    private static bool ValidateBlankOnly(IExpression original, IExpression solvedExp, ExpressionSolveContext context)
    {
        var schema = ExpressionSchemaFactory.Create(original.Length);

        for (int i = 0; i < schema.Length; i++)
        {
            if (original[i] == solvedExp[i]) continue; // 原题已有且未改 → 跳过

            if (string.IsNullOrWhiteSpace(solvedExp[i]))
                return false;

            if (schema.GetCellType(i) == CellType.Number)
            {
                var num = int.Parse(solvedExp[i]);
                if (!context.NumPool.Contains(num)) return false;
            }
            else if (schema.GetCellType(i) == CellType.Operator)
            {
                if (!SymbolManager.TryGetOpType(solvedExp[i], out var op) || 
                    !context.OpPool.Contains(op))
                    return false;
            }
        }
        return true;
    }

    // 模式2：所有格子都检查（包括原题已有的）
    private static bool ValidateFullPool(IExpression original, IExpression solvedExp, ExpressionSolveContext context)
    {
        var schema = ExpressionSchemaFactory.Create(original.Length);

        for (int i = 0; i < schema.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(solvedExp[i]))
                return false;

            if (schema.GetCellType(i) == CellType.Number)
            {
                var num = int.Parse(solvedExp[i]);
                if (!context.NumPool.Contains(num)) return false;
            }
            else if (schema.GetCellType(i) == CellType.Operator)
            {
                if (!SymbolManager.TryGetOpType(solvedExp[i], out var op) || 
                    !context.OpPool.Contains(op))
                    return false;
            }
        }
        return true;
    }

    // 模式3：严格消耗制（玩家的消耗不能超过池子中有的）
    private static bool ValidateDiscreteConsume(IExpression original, IExpression solvedExp, ExpressionSolveContext context)
    {
        var schema = ExpressionSchemaFactory.Create(original.Length);

        var usedNumbers = new List<int>();
        var usedOperators = new List<OpType>();

        for (int i = 0; i < schema.Length; i++)
        {
            string orig = original[i] ?? "";
            string solved = solvedExp[i] ?? "";

            // 原题已有且用户没改 → 不消耗
            if (!string.IsNullOrWhiteSpace(orig) && orig == solved)
                continue;

            if (string.IsNullOrWhiteSpace(solved))
                return false;

            if (schema.GetCellType(i) == CellType.Number)
            {
                var num = int.Parse(solved);
                usedNumbers.Add(num);
            }
            else if (schema.GetCellType(i) == CellType.Operator)
            {
                if (!SymbolManager.TryGetOpType(solved, out var op))
                    return false;
                usedOperators.Add(op);
            }
        }

        // 最终多重集精确相等
        return context.NumPool.IsValidMultiset(usedNumbers) && context.OpPool.IsValidMultiset(usedOperators);
    }
}