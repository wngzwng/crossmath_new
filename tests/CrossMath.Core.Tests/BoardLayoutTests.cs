using NUnit.Framework;
using CrossMath.Core.Models;
using CrossMath.Core.Types;
using System;
using System.IO;
using System.Linq;

namespace CrossMath.Core.Tests;

[TestFixture]
public class BoardLayoutTests
{
    private static readonly string[] Lines =
    {
        "1111101",
        "0000101",
        "1111101",
        "1010101",
        "1011111",
        "1010000",
        "1011111",
    };

    private const int W = 7;
    private const int H = 7;

    private BoardLayout _layout;
    private string _layoutStr;   // 拼接后的串

    [SetUp]
    public void SetUp()
    {
        _layout = BoardLayoutExtensions.FromLines(Lines);
        _layoutStr = string.Join("", Lines);
    }

    // ============================================================
    // 1) 构造 / 基础属性
    // ============================================================

    [Test]
    public void BoardLayout_ShouldHaveCorrectDimensions()
    {
        Assert.That(_layout.Width, Is.EqualTo(W));
        Assert.That(_layout.Height, Is.EqualTo(H));
        Assert.That(_layout.Length, Is.EqualTo(W * H));
    }

    [Test]
    public void ValidCellCount_ShouldBeCorrect()
    {
        int expected = _layoutStr.Count(c => c == '1');
        Assert.That(_layout.ValidCellCount, Is.EqualTo(expected));
    }

    // ============================================================
    // 2) 索引器正确性
    // ============================================================

    [Test]
    public void Indexer_OneDim_ShouldMatchUnderlyingString()
    {
        for (int i = 0; i < _layout.Length; i++)
            Assert.That(_layout[i], Is.EqualTo(_layoutStr[i].ToString()));
    }

    [Test]
    public void Indexer_TwoDim_ShouldMatchUnderlyingString()
    {
        for (int r = 0; r < H; r++)
            for (int c = 0; c < W; c++)
                Assert.That(_layout[r, c], Is.EqualTo(_layoutStr[r * W + c].ToString()));
    }

    // ============================================================
    // 3) IsValid 测试
    // ============================================================

    [Test]
    public void IsValid_ShouldReturnCorrectResult()
    {
        for (int r = 0; r < H; r++)
        {
            for (int c = 0; c < W; c++)
            {
                bool expected = _layoutStr[r * W + c] == '1';
                Assert.That(_layout.IsValid(r, c), Is.EqualTo(expected));
            }
        }
    }

    // ============================================================
    // 4) struct Enumerator（foreach）
    // ============================================================

    [Test]
    public void Enumerator_ShouldReturnAllValidPositions()
    {
        var positions = _layout.ToList();

        var expected = Enumerable.Range(0, _layout.Length)
            .Where(i => _layoutStr[i] == '1')
            .Select(i => new RowCol(i / W, i % W))
            .ToList();

        Assert.That(positions.Count, Is.EqualTo(expected.Count));
        Assert.That(positions, Is.EquivalentTo(expected));
    }

    // ============================================================
    // 5) Contains
    // ============================================================

    [Test]
    public void Contains_ShouldMatchIsValid()
    {
        foreach (var pos in _layout)
        {
            Assert.That(_layout.Contains(pos), Is.EqualTo(true));
        }
    }

    // ============================================================
    // 6) Equals / HashCode
    // ============================================================

    [Test]
    public void Equality_ShouldWork()
    {
        var layout2 = BoardLayoutExtensions.FromLines(Lines);

        Assert.That(_layout == layout2, Is.True);
        Assert.That(_layout.Equals(layout2), Is.True);
        Assert.That(_layout.GetHashCode(), Is.EqualTo(layout2.GetHashCode()));
    }

    [Test]
    public void Inequality_ShouldWork()
    {
        var different = BoardLayoutExtensions.FromLines(
            "1111111",
            "0000000",
            "1111111",
            "0000000",
            "1111111",
            "0000000",
            "1111111"
        );

        Assert.That(_layout != different, Is.True);
    }

    // ============================================================
    // 7) ToString
    // ============================================================

    [Test]
    public void ToString_ShouldContainBoardInformation()
    {
        string s = _layout.ToString();

        Assert.That(s, Does.Contain("BoardLayout 7×7"));
        Assert.That(s, Does.Contain($"{_layout.ValidCellCount}"));
        Assert.That(s, Does.Contain("1111101"));
    }

    // ============================================================
    // 8) FromLines 测试（再验证一次）
    // ============================================================

    [Test]
    public void FromLines_ShouldCreateCorrectLayout()
    {
        var layout = BoardLayoutExtensions.FromLines(Lines);
        Assert.That(layout.Width, Is.EqualTo(7));
        Assert.That(layout.Height, Is.EqualTo(7));
        Assert.That(layout.LayoutStr, Is.EqualTo(_layoutStr));
    }

    // ============================================================
    // 9) Print 测试（Console 捕获）
    // ============================================================

    [Test]
    public void Print_ShouldOutputBoardLayout()
    {
        var sw = new StringWriter();
        Console.SetOut(sw);

        _layout.Print();

        string output = sw.ToString();

        Assert.That(output, Does.Contain("BoardLayout 7×7"));
        Assert.That(output, Does.Contain("1111101"));
        Assert.That(output, Does.Contain("0000101"));
    }
}
