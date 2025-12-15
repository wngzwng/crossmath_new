namespace CrossMath.Core.Analytics.FinalBoard;

public class FinalBoardBrief
{
    /// <summary>终盘面板（最终表达式填满后的布局）</summary>
    public string EndInfo { get; set; } = "";

    /// <summary>面板掩码（BoardLayout 的 LayoutStr）</summary>
    public string LayoutInfo { get; set; } = "";

    /// <summary>全局最小数字</summary>
    public int MinValue { get; set; }

    /// <summary>全局最大数字</summary>
    public int MaxValue { get; set; }

    /// <summary>全局数字友好度（你自定义的 friendly metric）</summary>
    public double TotalFriendly { get; set; }

    /// <summary>全局运算符数量（按 + - × ÷ 顺序）</summary>
    public string OperatorGroup { get; set; } = "";

    /// <summary>7 长度算式的运算符分布（按 + - × ÷ 顺序）</summary>
    public string OperatorGroup7 { get; set; } = "";

    /// <summary>最大相同算式数量（弱算式语义）</summary>
    public int MaxSameFormulaNum { get; set; }

    /// <summary>乘 1 或 除 1 的算式数量</summary>
    public int NumMulDivBy1 { get; set; }
    
    public bool HasOneTwoMixOperatorInExp7 {get; set;}

    public override string ToString()
    {
        return
            $"LayoutInfo: {LayoutInfo}\n" +
            $"EndInfo: {EndInfo}\n" +
            $"MinValue: {MinValue}, MaxValue: {MaxValue}\n" +
            $"TotalFriendly: {TotalFriendly:F3}\n" +
            $"OperatorGroup: {OperatorGroup}\n" +
            $"OperatorGroup7: {OperatorGroup7}\n" +
            $"MaxSameFormulaNum: {MaxSameFormulaNum}\n" +
            $"NumMulDivBy1: {NumMulDivBy1}\n" + 
            $"HasOneTwoMixOperatorInExp7: {HasOneTwoMixOperatorInExp7}";
    }
    
    // ============================================================
    //  ToDict
    // ============================================================
    public Dictionary<string, string> ToDict()
    {
        return new Dictionary<string, string>
        {
            ["end_info"]          = EndInfo,
            ["layout_info"]       = LayoutInfo,
            ["min_value"]         = MinValue.ToString(),
            ["max_value"]         = MaxValue.ToString(),
            ["total_friendly"]    = TotalFriendly.ToString("F2"),
            ["operator_group"]    = OperatorGroup,
            ["operator_7_group"]  = OperatorGroup7,
            ["same_formula_num"]  = MaxSameFormulaNum.ToString(),
            ["num_m_and_d_1"]     = NumMulDivBy1.ToString(),
            ["has_one_two_mix_operator_in_exp7"] = HasOneTwoMixOperatorInExp7.ToString(),
        };
    }

    // ============================================================
    //  FromDict
    // ============================================================
    public static FinalBoardBrief FromDict(Dictionary<string, string> dict)
    {
        return new FinalBoardBrief
        {
            EndInfo           = dict.GetValueOrDefault("end_info", ""),
            LayoutInfo        = dict.GetValueOrDefault("layout_info", ""),
            MinValue          = int.TryParse(dict.GetValueOrDefault("min_value"), out var min) ? min : 0,
            MaxValue          = int.TryParse(dict.GetValueOrDefault("max_value"), out var max) ? max : 0,
            TotalFriendly     = double.TryParse(dict.GetValueOrDefault("total_friendly"), out var f) ? f : 0.0,
            OperatorGroup     = dict.GetValueOrDefault("operator_group", ""),
            OperatorGroup7    = dict.GetValueOrDefault("operator_7_group", ""),
            MaxSameFormulaNum = int.TryParse(dict.GetValueOrDefault("same_formula_num"), out var sf) ? sf : 0,
            NumMulDivBy1      = int.TryParse(dict.GetValueOrDefault("num_m_and_d_1"), out var md1) ? md1 : 0,
            HasOneTwoMixOperatorInExp7 = int.TryParse(dict.GetValueOrDefault("has_one_two_mix_operator_in_exp7"), out var n) && n == 1
        };
    }
}