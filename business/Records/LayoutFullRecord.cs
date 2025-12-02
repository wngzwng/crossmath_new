using CrossMath.Core.Analytics.EmptyBoard;

namespace business.Records;

using CrossMath.Core.Types;

/// <summary>
/// 布局的完整统计数据（Full Statistics Record）
/// 包含布局几何结构、算式数量、难度指标（sigma）、边缘/次边缘统计、Z 型算式数量等。
/// 用于数据分析、筛选、高级难度判定、训练模型等用途。
/// </summary>
public record LayoutFullRecord
{
    public int Id { get; set; }
    /// <summary>
    /// 盘面尺寸（Width × Height）
    /// </summary>
    public Size Size { get; set; }

    /// <summary>
    /// 盘面掩码（layout mask），即 BoardLayout 的二进制/字符串表示。
    /// 通常对应 LayoutStr：哪里是有效格子（1），哪里是空（0）。
    /// </summary>
    public string LayoutInfo { get; set; } = string.Empty;

    /// <summary>
    /// 全部算式数量（所有长度的算式总计）
    /// </summary>
    public int FormulaCount { get; set; }

    /// <summary>
    /// 长度为 7 的算式数量（中级/高级难度布局的重要指标）
    /// </summary>
    public int Formula7Count { get; set; }

    /// <summary>
    /// 环结构数量（ring number）
    /// 基于布局连通性分析，用于判断复杂度与区域分布结构。
    /// </summary>
    public int RingCount { get; set; }

    /// <summary>
    /// 布局整体难度 sigma（根据布局特征、算式数量、分布统计等得出的综合评分）
    /// </summary>
    public double Sigma { get; set; }

    /// <summary>
    /// 交点数量（cross count）
    /// 表示算式之间的交叉点数，是布局复杂度的关键因素。
    /// </summary>
    public int CrossCount { get; set; }

    /// <summary>
    /// 算式覆盖率（公式覆盖的格子数 / 有效格子数）
    /// </summary>
    public double FormulaCoverage { get; set; }

    /// <summary>
    /// Z 型算式数量（formula-Z count）
    /// 通常指算式布局呈现 Z 字型的情况，用于高级难度判断。
    /// </summary>
    public int FormulaZCount { get; set; }

    /// <summary>
    /// 最外层（outermost ring）顶部的格子数量
    /// </summary>
    public int OutermostTop { get; set; }

    /// <summary>
    /// 最外层底部的格子数量
    /// </summary>
    public int OutermostBottom { get; set; }

    /// <summary>
    /// 最外层左侧的格子数量
    /// </summary>
    public int OutermostLeft { get; set; }

    /// <summary>
    /// 最外层右侧的格子数量
    /// </summary>
    public int OutermostRight { get; set; }

    /// <summary>
    /// 最外层圈（outermost ring）的 sigma 值
    /// 表示布局边缘密度或复杂度，一般用于评估外层结构是否过稀或过密。
    /// </summary>
    public double SigmaOutermost { get; set; }

    /// <summary>
    /// 次外层（second outer ring）顶部格子数
    /// </summary>
    public int OuterTop { get; set; }

    /// <summary>
    /// 次外层底部格子数
    /// </summary>
    public int OuterBottom { get; set; }

    /// <summary>
    /// 次外层左侧格子数
    /// </summary>
    public int OuterLeft { get; set; }

    /// <summary>
    /// 次外层右侧格子数
    /// </summary>
    public int OuterRight { get; set; }
}
