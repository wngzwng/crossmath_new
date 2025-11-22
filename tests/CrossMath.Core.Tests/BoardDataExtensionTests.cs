using NUnit.Framework;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Tests;

[TestFixture]
public class BoardDataExtensionsTests
{
    private const string LayoutInfo = "1111100100010010111111010101101010100100010010001";
    private const int W = 7;
    private const int H = 7;

    private BoardData CreateEmptyBoard()
    {
        return BoardData.Create(LayoutInfo, W, H);
    }

    // ---------- GetKind ----------
    [Test]
    public void GetKind_ShouldReturn_LayoutOnly_WhenBoardIsEmpty()
    {
        var board = CreateEmptyBoard();
        Assert.That(board.GetKind(), Is.EqualTo(BoardKind.LayoutOnly));
    }

    [Test]
    public void GetKind_ShouldReturn_Answer_WhenBoardIsFullyFilled()
    {
        var board = CreateEmptyBoard();

        foreach (var pos in board.ValidPositions)
            board.SetValueOnly(pos, "1"); // 填满

        Assert.That(board.GetKind(), Is.EqualTo(BoardKind.Answer));
    }
    
    [Test]
    public void GetKind_ShouldReturn_AnswerDraft_WhenSomeFilled_WithoutAnswers()
    {
        var board = CreateEmptyBoard();

        var first = board.ValidPositions.First();
        board.SetValueOnly(first, "5");

        Assert.That(board.GetKind(), Is.EqualTo(BoardKind.AnswerDraft));
    }


    // ---------- ToReadableString ----------
    [Test]
    public void ToReadableString_Should_NotThrow()
    {
        var board = CreateEmptyBoard();

        string repr = board.ToReadableString();

        Assert.That(repr, Is.Not.Null.And.Not.Empty);
        TestContext.WriteLine(repr);
    }


    // ---------- GetSortedHoles ----------
    [Test]
    public void GetSortedHoles_ShouldBeOrdered_ByRowAndCol()
    {
        var board = CreateEmptyBoard();

        var sorted = board.GetSortedHoles();

        // 校验升序
        for (int i = 1; i < sorted.Count; i++)
        {
            var prev = sorted[i - 1];
            var curr = sorted[i];

            Assert.That(
                curr.Row > prev.Row || (curr.Row == prev.Row && curr.Col > prev.Col),
                Is.True,
                "必须按 Row 再 Col 升序排序");
        }
    }
}
