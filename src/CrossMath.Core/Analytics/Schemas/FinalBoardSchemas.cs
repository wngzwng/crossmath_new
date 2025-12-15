using CrossMath.Core.Analytics.Fields;
namespace CrossMath.Core.Analytics.Schemas;

public static class FinalBoardSchemas
{
    public static readonly Schema Full = new(
        "final_board_full",
        new IField[]
        {
            FieldDef.EndInfo,
            FieldDef.LayoutInfo,

            FieldDef.MinValue,
            FieldDef.MaxValue,
            FieldDef.TotalFriendly,

            FieldDef.OperatorGroup,
            FieldDef.OperatorGroup7,

            FieldDef.MaxSameFormulaNum,
            FieldDef.NumMulDivBy1,
            FieldDef.HasOneTwoMixOperatorInExp7,
        }
    );
}
