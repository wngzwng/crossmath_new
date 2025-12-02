using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Models;
using CrossMath.Core.Types;
using CrossMath.Core.Utils;

namespace CrossMath.Core.Analytics.EmptyBoard;

public  class EmptyBoardAnalyzer
{
    public static EmptyBoardBrief GetInfo(BoardLayout layout)
    {
        return new EmptyBoardAnalyzer().Analyze(layout);
    }
    
    public EmptyBoardBrief Analyze(BoardLayout layout)
    {
        var explayouts = ExtraExpLayouts(layout).ToList();
        var formulaCount = FormulaCount(explayouts);
        var crossCount = CrossCount(layout);

        var outerMost = GetOutermosts(layout);
        var outer = GetOuter(layout);

        var sigmaOuterMost = MathMisc.CalcSigma(outerMost.Values.ToList());
        return new EmptyBoardBrief()
        {
            Size = layout.BoardSize,
            LayoutInfo = layout.LayoutStr,

            FormulaCount = FormulaCount(explayouts),
            Formula7Count = Formula7Count(explayouts),
            RingCount = RingCount(crossCount, formulaCount),
            Sigma = Sigma(layout),
            CrossCount = crossCount,
            FormulaCoverage = FormulaCoverage(layout),
            FormulaZCount = SpecialExpressionAnalyzer.FromLayout(layout, [5, 7]).CountZShape(),

            OutermostTop = outerMost[OuterMostNames.OutermostTop],
            OutermostBottom = outerMost[OuterMostNames.OutermostBottom],
            OutermostLeft = outerMost[OuterMostNames.OutermostLeft],
            OutermostRight = outerMost[OuterMostNames.OutermostRight],
            SigmaOutermost = sigmaOuterMost,

            OuterTop = outer[OuterNames.OuterTop],
            OuterBottom = outer[OuterNames.OuterBottom],
            OuterLeft = outer[OuterNames.OuterLeft],
            OuterRight = outer[OuterNames.OuterRight],
        };
    }

    public double Sigma(BoardLayout layout) => layout.Sigma();

    public int FormulaCount(IEnumerable<ExpressionLayout> expLayouts) => expLayouts.Count();
    public int FormulaCount(BoardLayout layout) => FormulaCount(ExtraExpLayouts(layout));
    
    public int Formula7Count(IEnumerable<ExpressionLayout> expLayouts) => expLayouts.Count(expLay => expLay.Length == 7);
    public int Formula7Count(BoardLayout layout) => Formula7Count(ExtraExpLayouts(layout));

    public int CrossCount(BoardLayout layout)
    {
        return layout.ValidPositions().Count(pos => IsCross(layout, pos));
        
        bool IsCross(BoardLayout boardLayout, RowCol pos)
        {
            var size = boardLayout.BoardSize;
            if (!pos.InBounds(size)) return false;

            var hasUp = boardLayout.IsValid(pos + RowCol.Up);
            var hasDown = boardLayout.IsValid(pos + RowCol.Down);
        
            var hasLeft = boardLayout.IsValid(pos + RowCol.Left);
            var hasRight = boardLayout.IsValid(pos + RowCol.Right);
        
            return (hasUp || hasDown) && (hasLeft || hasRight);
        }
    }

    public int RingCount(int crossCount, int formulaCount)
    {

        return crossCount - (formulaCount - 1);
    }

    public int RingCount(BoardLayout layout)
    {
        return RingCount(CrossCount(layout), FormulaCount(ExtraExpLayouts(layout)));
    }

    public IEnumerable<ExpressionLayout> ExtraExpLayouts(BoardLayout layout)
    {
        return ExpressionLayoutBuilder.ExtractLayouts(layout, [5, 7]);
    }

    public double FormulaCoverage(BoardLayout layout)
    {
        return PuzzleCoverage.CalcCoverage(FormulaCount(layout), layout.BoardSize,
            FormulaCapacityRegistry.CreateDefaultRegistry());
    }
    
    
    public static class CornerNames
    {
        public const string TopLeft = "TopLeft";
        public const string TopRight = "TopRight";
        public const string BottomLeft = "BottomLeft";
        public const string BottomRight = "BottomRight";
    }
    
    public Dictionary<string, RowCol> GetCorners(Size size)
    {
        var corners = new Dictionary<string, RowCol>();
    
        // 左上角
        corners[CornerNames.TopLeft] = RowCol.At(0, 0);
        // 右上角
        corners[CornerNames.TopRight] = RowCol.At(0, size.Width - 1);
        // 左下角
        corners[CornerNames.BottomLeft] = RowCol.At(size.Height - 1, 0);
        // 右下角
        corners[CornerNames.BottomRight] = RowCol.At(size.Height - 1, size.Width - 1);
    
        return corners;
    }


    public static class OuterMostNames
    {
        public const string OutermostTop = "OutermostTop";
        public const string OutermostBottom = "OutermostBottom";
        public const string OutermostLeft = "OutermostLeft";
        public const string OutermostRight = "OutermostRight";
    }
    
    public static class OuterNames
    {
        public const string OuterTop = "OutermostTop";
        public const string OuterBottom = "OutermostBottom";
        public const string OuterLeft = "OutermostLeft";
        public const string OuterRight = "OutermostRight";
    }
    
    private int CountValidCells(BoardLayout layout, RowCol start, RowCol end)
        => Size.TraverseSection(start, end).Count(layout.IsValid);

    public Dictionary<string, int> GetOutermosts(BoardLayout layout)
    {
        var corners = GetCorners(layout.BoardSize);

        return new Dictionary<string, int>()
        {
            [OuterMostNames.OutermostTop] = CountValidCells(layout, corners[CornerNames.TopLeft], corners[CornerNames.TopRight]),
            [OuterMostNames.OutermostBottom] = CountValidCells(layout, corners[CornerNames.BottomLeft], corners[CornerNames.BottomRight]),
            [OuterMostNames.OutermostLeft] = CountValidCells(layout, corners[CornerNames.TopLeft], corners[CornerNames.BottomLeft]),
            [OuterMostNames.OutermostRight] = CountValidCells(layout, corners[CornerNames.TopRight], corners[CornerNames.BottomRight]),
        };
    }

    public Dictionary<string, int> GetOuter(BoardLayout layout)
    {
        var corners = GetCorners(layout.BoardSize);
    
        return new Dictionary<string, int>()
        {
            [OuterNames.OuterTop] = CountValidCells(layout, 
                corners[CornerNames.TopLeft] + RowCol.At(1, 1), 
                corners[CornerNames.TopRight] + RowCol.At(1, -1)),
            [OuterNames.OuterBottom] = CountValidCells(layout, 
                corners[CornerNames.BottomLeft] + RowCol.At(-1, 1), 
                corners[CornerNames.BottomRight] + RowCol.At(-1, -1)),
            [OuterNames.OuterLeft] = CountValidCells(layout, 
                corners[CornerNames.TopLeft] + RowCol.At(1, 1), 
                corners[CornerNames.BottomLeft] + RowCol.At(1, -1)),
            [OuterNames.OuterRight] = CountValidCells(layout, 
                corners[CornerNames.TopRight] + RowCol.At(-1, 1), 
                corners[CornerNames.BottomRight] + RowCol.At(-1, -1)),
        };
    }
    
    
}