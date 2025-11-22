namespace CrossMath.Core.Types;

/// <summary>
/// 盘面的粗粒度用途分类，用于组件路由与流程分发。
/// 设计原则：互斥、覆盖全部、判断成本为零、从名称即可理解权限和用途。
/// </summary>
public enum BoardKind : byte
{
    /// <summary>
    /// 【结构盘】只有算式结构，无任何数字。
    /// 用途：仅作为 Filler 的输入。
    /// 典型来源：空盘生成器。
    /// </summary>
    LayoutOnly = 0,

    /// <summary>
    /// 【答案草盘】部分数字已填，但尚未填满，且没有候选数。
    /// 典型来源：Filler 填充过程的中间态或内部截取的残盘。
    /// 用途：仅能继续填充，不对外使用。
    /// </summary>
    AnswerDraft = 1,

    /// <summary>
    /// 【答案盘】所有合法格子均已填满，可作为完整终盘。
    /// 无求解过程含义，仅表示“这是一个完整答案”。
    /// 用途：可挖空（生成题面）、可作为标准答案。
    /// </summary>
    Answer = 2,

    /// <summary>
    /// 【题面盘】包含至少一个空格，并且为每个空格都提供了对应数量的“备选答案”
    ///（Possible Answers）。备选答案是否正确属于求解层面的判断，
    /// 不影响本盘被归类为题面盘。
    ///
    /// 用途：可求解、可交互、可评分。
    /// </summary>
    Problem = 3,
    
    
    /// <summary>
    /// 无法分类或数据损坏。
    /// </summary>
    Unknown = 255
}
