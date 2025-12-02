using CrossMath.Core.Types;

namespace business.works.Layout;

// 初始放置点生成
public class InitialPlacementGenerator
{
    /*
     * 盘面大小
     * 算式长度
     * 焦点位置
     */

    public IEnumerable<Placement> Generate(Size size, int expressionLength, CrossType crossType)
    {
        return crossType switch
        {
            CrossType.Number => GenerateCrossNumber(size, expressionLength),
            CrossType.Operator => GenerateCrossOperator(size, expressionLength),
            CrossType.Equal => GenerateEqual(size, expressionLength),
            _ => Enumerable.Empty<Placement>()
        };
    }
    
    // 数字焦点处理
    private IEnumerable<Placement> GenerateCrossNumber(Size size, int expressionLength)
    {
        if (size.Width < expressionLength || size.Height < 1) yield break;
        // 1. 在第0行，横向放置算式（向右延伸）
        // 起始列从 0 开始，每隔 2 格尝试（避免重叠）
        for (int col = 0; col <= size.Width - expressionLength; col += 2)
        {
            yield return new Placement(0, col, Direction.Horizontal, expressionLength);
        }

        // 2. 在第0行，纵向放置算式（向下延伸）
        // 起始位置是 (0, col)，方向垂直
        // 注意：纵向算式需要检查高度是否足够
        if (size.Height >= expressionLength)
        {
            for (int col = 0; col < size.Width; col += 2)
            { 
                yield return new Placement(0, col, Direction.Vertical, expressionLength);
            }
        }
    }

    private IEnumerable<Placement> GenerateCrossOperator(Size size, int expressionLength)
    {
        // 1. 在第1行，横向放置算式（向右延伸）
        // 起始列从 0 开始，每隔 2 格尝试（避免重叠）
        if (size.Height > 1)
        {
            for (int col = 0; col <= size.Width - expressionLength; col += 2)
            {
                yield return new Placement(1, col, Direction.Horizontal, expressionLength);
            }
        }
        
        // 2. 在第0行，纵向放置算式（向下延伸）
        // 起始位置是 (0, col)，方向垂直
        // 注意：纵向算式需要检查高度是否足够
        if (size.Height >= expressionLength && size.Width > 1)
        {
            for (int col = 1; col <= size.Width - 3; col += 2)
            { 
                yield return new Placement(0, col, Direction.Vertical, expressionLength);
            }
        }
    }
    
    /// <summary>
    /// 等号焦点模式（现代主流布局）
    /// 特点：
    /// - 横向算式从第1行开始
    /// - 等号固定在倒数第二格：例如 length=5 → 格子索引：0 1 2 3 = 4
    /// - 纵向算式从第0行开始，其起始列必须精确对齐「等号列」
    /// - 横向起始列步长为 2（最密布局）或 4（最美观对齐），推荐步长 2 + 等号列对齐
    /// </summary>
    private IEnumerable<Placement> GenerateEqual(Size size, int expressionLength)
    {
        if (size.Width < expressionLength || size.Height < 2) yield break;

        int equalOffset = expressionLength - 2;  // 等号在倒数第二格

        // 1. 横向算式：从第1行开始，起始列每隔 2 格（最大化覆盖）
        if (size.Height >= equalOffset)
        {
            for (int col = 0; col <= size.Width - expressionLength; col += 2)
            {
                int equalCol = col + equalOffset;

                // 保证等号不越界（虽然 <= width-expressionLength 已保证，但防御性编程）
                if (equalCol < size.Width)
                {
                    yield return new Placement(1, col, Direction.Horizontal, expressionLength);
                }
            }
        }

        // 2. 纵向算式：起始列必须等于「横向算式的等号列」
        //    即：col = 横向起始列 + (expressionLength - 2)
        //    因为横向起始列是 0,2,4,... → 等号列是 (expressionLength-2), (expressionLength-2)+2, ...
        if (size.Height >= expressionLength)
        {
            int firstEqualCol = 0 + equalOffset - 1;           // 第一个横向算式的等号列
            int step = 2;                                  // 与横向步长一致

            for (int col = firstEqualCol; col <= size.Width - 2; col += step)
            {
                yield return new Placement(0, col, Direction.Vertical, expressionLength);
            }
        }
    }


    // ==========================================================
    // 多配置合并（Set 语义）
    // ==========================================================
    public static HashSet<Placement> BuildPlacement(IEnumerable<(Size size, int length, CrossType type)> rules)
    {
        var gen = new InitialPlacementGenerator();
        var result = new HashSet<Placement>();

        foreach (var (size, length, type) in rules)
        foreach (var p in gen.Generate(size, length, type))
            result.Add(p);

        return result;
    }
    
}