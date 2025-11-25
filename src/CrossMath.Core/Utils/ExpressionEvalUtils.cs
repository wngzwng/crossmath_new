using CrossMath.Core.Types;

namespace CrossMath.Core.Utils;

/// <summary>
/// 表达式求值工具（支持中缀 -> RPN，RPN求值，浮点数）
/// </summary>
public static class ExpressionEvalUtils
{
    // ------------------------------------------------------------
    // 1. 逆波兰式求值（支持浮点）
    // ------------------------------------------------------------
    public static bool TryComputeRpnDouble(
        IReadOnlyList<object> rpn,
        out double result)
    {
        var stack = new Stack<double>();

        foreach (var token in rpn)
        {
            switch (token)
            {
                case int i:
                    stack.Push(i);
                    break;

                case double d:
                    stack.Push(d);
                    break;

                case OpType op:
                    if (stack.Count < 2)
                    {
                        result = double.NaN;
                        return false;
                    }

                    double b = stack.Pop();
                    double a = stack.Pop();

                    double c = op switch
                    {
                        OpType.Add => a + b,
                        OpType.Sub => a - b,
                        OpType.Mul => a * b,
                        OpType.Div => b != 0 ? a / b : double.NaN,
                        _ => double.NaN
                    };

                    if (double.IsNaN(c))
                    {
                        result = double.NaN;
                        return false;
                    }

                    stack.Push(c);
                    break;

                default:
                    result = double.NaN;
                    return false;
            }
        }

        if (stack.Count == 1)
        {
            result = stack.Pop();
            return true;
        }

        result = double.NaN;
        return false;
    }


    // ------------------------------------------------------------
    // 2. 将中缀表达式转换为 RPN（逆波兰）
    // 输入：数字序列 nums，运算符序列 ops
    // 要求：nums.Count = ops.Count + 1
    // ------------------------------------------------------------
    public static IReadOnlyList<object> ToRpn(
        IReadOnlyList<double> nums,
        IReadOnlyList<OpType> ops)
    {
        if (nums.Count != ops.Count + 1)
            throw new ArgumentException("中缀表达式不合法：nums.Count 必须 = ops.Count + 1");

        // Shunting Yard 算法：数字直接输出，运算符根据优先级输出
        List<object> output = new();
        Stack<OpType> opStack = new();

        // 输出第一个数字
        output.Add(nums[0]);

        for (int i = 0; i < ops.Count; i++)
        {
            var op = ops[i];

            // 当前操作符的优先级
            int prec = GetPrecedence(op);

            // 弹出所有优先级 >= 当前op 的操作符
            while (opStack.Count > 0 && GetPrecedence(opStack.Peek()) >= prec)
                output.Add(opStack.Pop());

            opStack.Push(op);

            // 输出下一个数字
            output.Add(nums[i + 1]);
        }

        // 弹出剩余操作符
        while (opStack.Count > 0)
            output.Add(opStack.Pop());

        return output;
    }
    
    public static bool IsInteger(double x)
    {
        return Math.Abs(x - Math.Round(x)) < 1e-9;
    }



    // ------------------------------------------------------------
    // 工具：获取运算符优先级
    // ------------------------------------------------------------
    private static int GetPrecedence(OpType op) =>
        op switch
        {
            OpType.Mul => 2,
            OpType.Div => 2,
            OpType.Add => 1,
            OpType.Sub => 1,
            _ => 0
        };
}

