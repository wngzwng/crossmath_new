using CrossMath.Core.Types;
using CrossMath.Core.Models;
using CrossMath.Core.Utils;

namespace CrossMath.Core.Expressions.Core;

public class Expression7 : BaseExpression
{
    public int? A { get; set; }
    public OpType? Op1 { get; set; }
    public int? B { get; set; }
    public OpType? Op2 { get; set; }
    public int? C { get; set; }
    public int? T { get; set; }  // 最终结果

    public Expression7() : base(7) { }

    // ===================================================================
    //  Fully Filled
    // ===================================================================
    public override bool IsFullyFilled =>
        A.HasValue && B.HasValue && C.HasValue && T.HasValue &&
        Op1 is not null and not OpType.Unspecified and not OpType.Invalid &&
        Op2 is not null and not OpType.Unspecified and not OpType.Invalid;

    // ===================================================================
    //  Token Getter
    // ===================================================================
    protected override string GetTokenCore(int index) => index switch
    {
        0 => A?.ToString() ?? string.Empty,
        1 => SymbolManager.GetSymbolOrDefault(Op1, string.Empty),
        2 => B?.ToString() ?? string.Empty,
        3 => SymbolManager.GetSymbolOrDefault(Op2, string.Empty),
        4 => C?.ToString() ?? string.Empty,
        5 => SymbolManager.SymbolEqual,  // "="
        6 => T?.ToString() ?? string.Empty,
        _ => throw new IndexOutOfRangeException()
    };

    // ===================================================================
    //  Token Setter
    // ===================================================================
    protected override void SetTokenCore(int index, string value)
    {
        switch (index)
        {
            case 0:
                A = ParseNullableInt(value);
                break;

            case 1:
                Op1 = SymbolManager.TryGetOpType(value, out var opA) ? opA : null;
                break;

            case 2:
                B = ParseNullableInt(value);
                break;

            case 3:
                Op2 = SymbolManager.TryGetOpType(value, out var opB) ? opB : null;
                break;

            case 4:
                C = ParseNullableInt(value);
                break;

            case 5:
                // "=" 固定位置，不解析
                break;

            case 6:
                T = ParseNullableInt(value);
                break;

            default:
                throw new IndexOutOfRangeException();
        }
    }

    // ===================================================================
    //  Evaluate
    // ===================================================================
    public override bool Evaluate()
    {
        if (!IsFullyFilled)
            return false;

        // 1. 构建 nums 数组（double）
        var nums = new double[]
        {
            A!.Value,
            B!.Value,
            C!.Value
        };

        // 2. 构建 ops 数组
        var ops = new OpType[]
        {
            Op1!.Value,
            Op2!.Value
        };

        // 3. 转换为 RPN（逆波兰表达式）
        var rpn = ExpressionEvalUtils.ToRpn(nums, ops);

        // 4. 求值
        if (!ExpressionEvalUtils.TryComputeRpnDouble(rpn, out var value))
            return false;

        // 5. 与 T 比较
        return Math.Abs(value - T!.Value) < 1e-9;
    }


    // ===================================================================
    // Clone
    // ===================================================================
    public override IExpression Clone()
        => new Expression7()
        {
            A = this.A,
            Op1 = this.Op1,
            B = this.B,
            Op2 = this.Op2,
            C = this.C,
            T = this.T
        };

    // ===================================================================
    // Debug
    // ===================================================================
    public override string ToString() =>
        $"{A?.ToString() ?? "□"} " +
        $"{SymbolManager.GetSymbolOrDefault(Op1, "□")} " +
        $"{B?.ToString() ?? "□"} " +
        $"{SymbolManager.GetSymbolOrDefault(Op2, "□")} " +
        $"{C?.ToString() ?? "□"} = {T?.ToString() ?? "□"}";
}
