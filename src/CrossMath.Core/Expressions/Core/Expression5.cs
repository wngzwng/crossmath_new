using CrossMath.Core.Types;
using CrossMath.Core.Models;

namespace CrossMath.Core.Expressions.Core;
public class Expression5 : BaseExpression
{
    public int? A { get; set; }          // 左操作数
    public OpType? Op { get; set; }      // 运算符
    public int? B { get; set; }          // 右操作数
    public int? C { get; set; }          // 等号右边（结果）

    public Expression5() : base(5) { }

    public override bool IsFullyFilled =>
        A.HasValue && Op.HasValue && B.HasValue && C.HasValue;

    // ---------------------------------------------------------
    // Token 映射（你原来的逻辑这里全部覆盖）
    // ---------------------------------------------------------
    protected override string GetTokenCore(int index) => index switch
    {
        0 => A?.ToString() ?? string.Empty,
        1 => SymbolManager.GetSymbolOrDefault(Op, string.Empty),
        2 => B?.ToString() ?? string.Empty,
        3 => SymbolManager.SymbolEqual,
        4 => C?.ToString() ?? string.Empty,
        _ => throw new IndexOutOfRangeException()
    };

    protected override void SetTokenCore(int index, string value)
    {
        switch (index)
        {
            case 0:
                A = ParseNullableInt(value);
                break;

            case 1:
                Op = SymbolManager.TryGetOpType(value, out var op)
                    ? op
                    : null;
                break;

            case 2:
                B = ParseNullableInt(value);
                break;

            case 3:
                // "=" 固定位置，不处理
                break;

            case 4:
                C = ParseNullableInt(value);
                break;

            default:
                throw new IndexOutOfRangeException();
        }
    }

    // ---------------------------------------------------------
    // Evaluate（原样迁移）
    // ---------------------------------------------------------
    public override bool Evaluate()
    {
        if (!IsFullyFilled) return false;

        return Op switch
        {
            OpType.Add => (A + B) == C,
            OpType.Sub => (A - B) == C,
            OpType.Mul => (A * B) == C,
            OpType.Div =>
                B != 0 &&
                A % B == 0 &&
                (A / B) == C,

            _ => false
        };
    }

    // ---------------------------------------------------------
    // Clone（简单复制）
    // ---------------------------------------------------------
    public override IExpression Clone()
        => new Expression5()
        {
            A = this.A,
            Op = this.Op,
            B = this.B,
            C = this.C
        };
    
    public override string ToString() =>
        $"{A?.ToString() ?? "□"} " +
        $"{SymbolManager.GetSymbolOrDefault(Op, "□")} " +
        $"{B?.ToString() ?? "□"} = {C?.ToString() ?? "□"}";
}
