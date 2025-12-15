using System.Text;
using CrossMath.Core.Analytics.utils;
using CrossMath.Core.Codec;
using CrossMath.Core.Expressions;
using CrossMath.Core.Expressions.Core;
using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Expressions.Schema;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Analytics.FinalBoard;

public class FinalBoardAnalyzer
{
    public static FinalBoardBrief GetInfo(BoardData board)
    {
        return new FinalBoardAnalyzer().Analyze(board);
    }

    // ----------------------------
    // 主入口
    // ----------------------------
    public FinalBoardBrief Analyze(BoardData board)
    {
        var layout = board.Layout;
        
        var groupAll = OperatorGroup(board);
        var group7 = OperatorGroup(board, l => l.Length == 7);

        var (level, layoutStr) = BoardDataCodec.Encode(board);
        return new FinalBoardBrief
        {
            EndInfo = level,
            LayoutInfo = layoutStr,

            MinValue = MinValue(board),
            MaxValue = MaxValue(board),

            TotalFriendly = CalcFriendly(board.GetAllNumbers()),

            OperatorGroup = OperatorGroupToString(groupAll),
            OperatorGroup7 = OperatorGroupToString(group7),

            MaxSameFormulaNum = MaxSameFormulaCount(board),

            NumMulDivBy1 = CountMulDivBy1(board),
        };
    }

    // ----------------------------
    // 1. 最小数字
    // ----------------------------
    public int MinValue(BoardData board)
        => board.GetAllNumbers().Min();

    // ----------------------------
    // 2. 最大数字
    // ----------------------------
    public int MaxValue(BoardData board)
        => board.GetAllNumbers().Max();

    // ----------------------------
    // 3. 数字友好度（你可以根据需要替换）
    // ----------------------------
    public double CalcFriendly(IEnumerable<int> numbers)
    {
        return NumberFriendlyCalculator.CalcFriendly(numbers);
    }

    // ----------------------------
    // 4. 运算符计数
    // ----------------------------
    public Dictionary<string, int> OperatorGroup(BoardData board, Func<ExpressionLayout, bool>? predicate = null)
    {
        var operatorGroup = new Dictionary<string, int>();
        var expLayouts = ExpressionLayoutBuilder.ExtractLayouts(board.Layout, new[] { 5, 7 });

        if (predicate != null)
            expLayouts = expLayouts.Where(predicate).ToList();

        foreach (var layout in expLayouts)
        {
            var exp = layout.ToExpression(board);
            var schema = ExpressionSchemaFactory.Create(exp.Length);
            var cellTypes = schema.GetAllCellTypes();

            for (int i = 0; i < cellTypes.Count; i++)
            {
                if (cellTypes[i] == CellType.Operator)
                {
                    var op = exp[i];
                    if (SymbolManager.AllOperatorSymbols.Contains(op))
                    {
                        operatorGroup[op] = operatorGroup.GetValueOrDefault(op) + 1;
                    }
                }
            }
        }

        return operatorGroup;
    }

    // 顺序：+ , - , * , /
    public string OperatorGroupToString(Dictionary<string, int> operatorGroup)
    {
        string[] ops = 
        {
            SymbolManager.SymbolAdd,
            SymbolManager.SymbolSub,
            SymbolManager.SymbolMul,
            SymbolManager.SymbolDiv
        };

        int[] counts = ops.Select(op => operatorGroup.GetValueOrDefault(op, 0)).ToArray();

        return string.Join(",", counts);
    }

    // ----------------------------
    // 5. 最大重复算式
    // ----------------------------
    public int MaxSameFormulaCount(BoardData board)
    {
        var map = SameExpressionCounter.WeakCounts(board);
        return map.Values.Max();
    }

    // ----------------------------
    // 6. 乘 1 / 除 1 计数
    // ----------------------------
    public int CountMulDivBy1(BoardData board)
    {
        var expLayouts = ExpressionLayoutBuilder.ExtractLayouts(board.Layout, new[] { 5, 7 });
        int count = 0;

        foreach (var layout in expLayouts)
        {
            var exp = layout.ToExpression(board);

            // 判断表达式是否有 "*1" 或 "/1"
            if (WeakExpressionUtils.ContainsMulDivByOne(exp))
                count++;
        }

        return count;
    }


    public int CountOneTwoMinOperatorInExp7(BoardData board)
    {
        var expLayouts = ExpressionLayoutBuilder.ExtractLayouts(board.Layout, new[] { 5, 7 })
            .Where(expLayout => expLayout.Length == 7).ToList();
        if (expLayouts.Count == 0)
        {
            return 0;
        }
        
        var count = 0;
        foreach (var layout in expLayouts)
        {
            var exp = (Expression7)layout.ToExpression(board);
            var op1 = exp.Op1!;
            var op2 = exp.Op2!;
            
            // 加/减 = 0， 乘/除 = 1
            int g1 = (op1 == OpType.Add || op1 == OpType.Sub) ? 0 : 1;
            int g2 = (op2 == OpType.Add || op2 == OpType.Sub) ? 0 : 1;

            if (g1 != g2)
                count++;
            
        }

        return count;
    }
    
    public bool HasOneTwoMinOperatorInExp7(BoardData board) => CountOneTwoMinOperatorInExp7(board) > 0;
}
