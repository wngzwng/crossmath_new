using System;
using NUnit.Framework;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Tests;

[TestFixture]
public class BoardDataTests
{
    private const string LevelInfo  = "7701fd00fa09fdfb001ffc00fa00fafcfafb1300151afafa092d09130c1316";
    private const string LayoutInfo = "1111100100010010111111010101101010100100010010001";
    private const int W = 7;
    private const int H = 7;

    // ① layout_info 长度 与 宽高一致
    [Test]
    public void LayoutInfo_Length_ShouldMatch_WidthTimesHeight()
    {
        Assert.That(LayoutInfo.Length, Is.EqualTo(W * H));
    }

    // ② BoardLayout 构造正常，合法格子数 = '1' 的数量
    [Test]
    public void BoardLayout_Should_Construct_Correctly()
    {
        var layout = new BoardLayout(LayoutInfo, W, H);

        Assert.That(layout.Width,  Is.EqualTo(W));
        Assert.That(layout.Height, Is.EqualTo(H));

        var onesCount = LayoutInfo.Count(c => c == '1');
        Assert.That(layout.ValidCellCount, Is.EqualTo(onesCount));
    }

    // ③ BoardData.Create 在纯空盘场景下能正常构造，并被识别为 LayoutOnly
    [Test]
    public void BoardData_Empty_Should_Be_LayoutOnly()
    {
        var board = BoardData.Create(
            layoutStr: LayoutInfo,
            width:     W,
            height:    H
        );

        // 空盘的约束：无填值、无答案，但 Holes = 所有合法格
        Assert.That(board.FilledValues.Count,    Is.EqualTo(0));
        Assert.That(board.PossibleAnswers.Count, Is.EqualTo(0));
        Assert.That(board.Holes.Count,           Is.EqualTo(board.Layout.ValidCellCount));

        var kind = board.GetKind();
        Assert.That(kind, Is.EqualTo(BoardKind.LayoutOnly));
    }

    // ④ ToReadableString 不应抛异常，且返回非空字符串
    [Test]
    public void BoardData_ToReadableString_Should_Work()
    {
        var board = BoardData.Create(
            layoutStr: LayoutInfo,
            width:     W,
            height:    H
        );

        string repr = board.ToReadableString();

        Assert.That(repr, Is.Not.Null.And.Not.Empty);
        TestContext.WriteLine(repr); // 调试时方便看
    }

    // ⑤ level_info 是合法十六进制串（长度为偶数，可被解析）
    [Test]
    public void LevelInfo_Should_Be_ValidHex()
    {
        // 长度为偶数
        Assert.That(LevelInfo.Length % 2, Is.EqualTo(0));

        // .NET 5+ 才有 Convert.FromHexString；你是 C# 9，可以用
        byte[] bytes = Convert.FromHexString(LevelInfo);
        Assert.That(bytes.Length, Is.GreaterThan(0));
    }
}
