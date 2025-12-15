namespace CrossMath.Core.Analytics.Fields;


public static class FieldDef
{
    // ============================================================
    // EmptyBoard
    // ============================================================
    // ===== 基础信息 =====
    public static readonly Field<string> Size =
        new("size");

    public static readonly Field<string> LayoutInfo =
        new("layout_info");

    // ===== 算式统计 =====
    public static readonly Field<int> FormulaCount =
        new("formula_count");

    public static readonly Field<int> Formula7Count =
        new("formula_7_count");

    public static readonly Field<int> FormulaZCount =
        new("formula_z_count");

    // ===== 结构复杂度 =====
    public static readonly Field<int> RingCount =
        new("ring_count");

    public static readonly Field<int> CrossCount =
        new("cross_count");

    public static readonly Field<double> FormulaCoverage =
        new("formula_coverage");

    // ===== Sigma =====
    public static readonly Field<double> Sigma =
        new("sigma");

    public static readonly Field<double> SigmaOutermost =
        new("sigma_outermost");

    // ===== 外圈结构 =====
    public static readonly Field<int> OutermostTop =
        new("outermost_top");

    public static readonly Field<int> OutermostBottom =
        new("outermost_bottom");

    public static readonly Field<int> OutermostLeft =
        new("outermost_left");

    public static readonly Field<int> OutermostRight =
        new("outermost_right");

    public static readonly Field<int> OuterTop =
        new("outer_top");

    public static readonly Field<int> OuterBottom =
        new("outer_bottom");

    public static readonly Field<int> OuterLeft =
        new("outer_left");

    public static readonly Field<int> OuterRight =
        new("outer_right");
    
    
    // ============================================================
    // FinalBoard
    // ============================================================

    public static readonly Field<string> EndInfo =
        new("end_info");

    public static readonly Field<int> MinValue =
        new("min_value");

    public static readonly Field<int> MaxValue =
        new("max_value");

    public static readonly Field<double> TotalFriendly =
        new("total_friendly");

    public static readonly Field<string> OperatorGroup =
        new("operator_group");

    public static readonly Field<string> OperatorGroup7 =
        new("operator_7_group");

    public static readonly Field<int> MaxSameFormulaNum =
        new("same_formula_num");

    public static readonly Field<int> NumMulDivBy1 =
        new("num_m_and_d_1");

    public static readonly Field<bool> HasOneTwoMixOperatorInExp7 =
        new("has_one_two_mix_operator_in_exp7");
}