using CrossMath.Core.Analytics.Fields;
namespace CrossMath.Core.Analytics.Schemas;

public static class EmptyBoardSchemas
{
    /// <summary>
    /// 完整空盘分析 Schema
    /// 用于：数据采集 / CSV / Excel / 全量分析
    /// </summary>
    public static readonly Schema Full = new(
        "empty_board_full",
        new IField[]
        {
            // ===== 基础信息 =====
            FieldDef.Size,
            FieldDef.LayoutInfo,

            // ===== 算式统计 =====
            FieldDef.FormulaCount,
            FieldDef.Formula7Count,
            FieldDef.FormulaZCount,

            // ===== 结构复杂度 =====
            FieldDef.RingCount,
            FieldDef.CrossCount,
            FieldDef.FormulaCoverage,

            // ===== Sigma =====
            FieldDef.Sigma,
            FieldDef.SigmaOutermost,

            // ===== 外圈结构（最外层）=====
            FieldDef.OutermostTop,
            FieldDef.OutermostBottom,
            FieldDef.OutermostLeft,
            FieldDef.OutermostRight,

            // ===== 外圈结构（次外层）=====
            FieldDef.OuterTop,
            FieldDef.OuterBottom,
            FieldDef.OuterLeft,
            FieldDef.OuterRight,
        }
    );

    /// <summary>
    /// 轻量 Schema
    /// 用于：快速筛选 / CLI / Debug
    /// </summary>
    public static readonly Schema Light = new(
        "empty_board_light",
        new IField[]
        {
            FieldDef.Size,
            FieldDef.FormulaCount,
            FieldDef.Sigma,
        }
    );

    /// <summary>
    /// 结构分析 Schema
    /// 用于：布局结构特征分析（不关心算式细节）
    /// </summary>
    public static readonly Schema Structure = new(
        "empty_board_structure",
        new IField[]
        {
            FieldDef.Size,
            FieldDef.LayoutInfo,

            FieldDef.RingCount,
            FieldDef.CrossCount,

            FieldDef.OutermostTop,
            FieldDef.OutermostBottom,
            FieldDef.OutermostLeft,
            FieldDef.OutermostRight,

            FieldDef.OuterTop,
            FieldDef.OuterBottom,
            FieldDef.OuterLeft,
            FieldDef.OuterRight,
        }
    );
}