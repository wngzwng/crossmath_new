using CrossMath.Core.Analytics.Fields;

namespace CrossMath.Core.Analytics.Schemas;

public static class InitBoardSchemas
{
    public static readonly Schema Full = new(
        "init_board_full",
        new IField[]
        {
            // ===== 基础信息 =====
            FieldDef.StartInfo,
            FieldDef.Difficulty,
            FieldDef.EmptyCellCount,
            FieldDef.Answer,

            // ===== 难度区间 =====
            FieldDef.MinDifficulty,
            FieldDef.MaxDifficulty,
            FieldDef.TotalDifficulty,
            FieldDef.StartStaticDifficulty,

            // ===== 卡点分析 =====
            FieldDef.StuckNum,
            FieldDef.StuckPoints,
            FieldDef.FirstStuckPoint,
            FieldDef.FirstStuckPointPercent,

            // ===== 空格友好度 =====
            FieldDef.EmptyFriendly,

            // ===== 解题路径 / 随机评估 =====
            FieldDef.MaxWeightCoordinates,
            FieldDef.MaxWeightScore,
            FieldDef.RandomScore,
        }
    );
}