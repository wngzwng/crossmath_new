using CrossMath.Core.Types;
using CrossMath.Core.Evaluation.LevelDifficulty;

namespace CrossMath.Core.Evaluation.LevelDifficulty.ScoreStrategies;

/// <summary>
/// 默认评分策略（ScoreStrategy）
/// <br/>
/// score =
///     vision_factor * vision_score
///   + deepth_factor * deepth_score
///<br/>
/// 其中：<br/>
/// - vision_score：基于路径的“视觉跨度”，使用曼哈顿距离<br/>
/// - deepth_score：基于候选数规模 + 局部难度 + 数字复杂度的“推理深度”<br/>
///<br/>
/// 该策略【只负责评分】：<br/>
/// - 不做权重归一化<br/>
/// - 不负责最终坐标选择<br/>
/// - 不修改上下文状态<br/>
/// </summary>
public sealed class DefaultScoreStrategy : IScoreStrategy
{
    /// <summary>
    /// 路径（视觉）评分因子
    /// 控制“移动距离”在最终 score 中的权重
    /// 对应 Python 中的 vision_factor
    /// </summary>
    private readonly double _visionFactor;

    /// <summary>
    /// 深度（推理）评分因子
    /// 控制候选复杂度 / 难度在最终 score 中的权重
    /// 对应 Python 中的 deepth_factor
    /// </summary>
    private readonly double _deepthFactor;

    /// <summary>
    /// 构造一个默认评分策略
    /// </summary>
    /// <param name="visionFactor">
    /// 视觉（路径）评分权重
    /// </param>
    /// <param name="deepthFactor">
    /// 深度（推理）评分权重
    /// </param>
    /// <param name="initDefaultCoord">
    /// 初始默认坐标；不指定时使用 RowCol.Zero
    /// </param>
    public DefaultScoreStrategy(
        double visionFactor = 0.4,
        double deepthFactor = 1.0,
        RowCol? initDefaultCoord = null)
    {
        _visionFactor = visionFactor;
        _deepthFactor = deepthFactor;
    }

    /// <summary>
    /// 对所有候选坐标进行评分
    /// </summary>
    public IReadOnlyDictionary<RowCol, double> Score(
        LevelDifficultyContext ctx,
        RowCol lastCoord,
        IReadOnlyDictionary<RowCol, int> localDifficulty)
    {
        // 当前棋盘所有候选数中，不同数字的个数
        int candidateUniqueCount =
            ctx.WorkingBoard.PossibleAnswers
                .Distinct()
                .Count();
        
        // 当前棋盘候选数字的总数量（包含重复）
        int candidateCount =
            ctx.WorkingBoard.PossibleAnswers.Count;

        // average_number_of_digits：
        // 当前棋盘所有候选数字的平均位数
        // 用于粗略刻画“数字规模复杂度”
        double averageNumberOfDigits =
            ctx.WorkingBoard.PossibleAnswers.Count == 0
                ? 0
                : ctx.WorkingBoard.PossibleAnswers
                    .Average(n => n.ToString().Length);
        

        var result = new Dictionary<RowCol, double>(localDifficulty.Count);

        // 对每一个候选坐标计算 score
        foreach (var (coord, difficulty) in localDifficulty)
        {
            // 1️⃣ Vision score：路径跨度（曼哈顿距离）
            double visionScore =
                GetVisionScore(coord, lastCoord);

            // 2️⃣ Deepth score：推理深度（与候选规模和局部难度相关）
            double deepthScore =
                GetDeepthScore(
                    candidateUniqueCount,
                    difficulty,
                    averageNumberOfDigits,
                    candidateCount);

            // 3️⃣ 线性组合得到最终 score
            double score =
                _visionFactor * visionScore +
                _deepthFactor * deepthScore;

            result[coord] = score;
        }

        return result;
    }

    /// <summary>
    /// 计算视觉评分（Vision Score）
    ///
    /// 使用当前坐标与上一个坐标之间的曼哈顿距离：
    /// |Δrow| + |Δcol|
    /// </summary>
    private static double GetVisionScore(RowCol current, RowCol last)
    {
        return Math.Abs(current.Row - last.Row)
             + Math.Abs(current.Col - last.Col);
    }

    /// <summary>
    /// 计算推理深度评分（Deepth Score）
    ///
    /// 对应 Python 中的 _get_deepth_score：
    ///
    /// - 当 difficulty >= 4 时：
    ///   candiate_unique_count * difficulty * (candiate_count - 1) * average_number_of_digits
    ///
    /// - 否则：
    ///   candiate_unique_count * difficulty * average_number_of_digits
    ///
    /// 该分段规则来自你在 2025-09-13 的修订版本
    /// </summary>
    private static double GetDeepthScore(
        int candidateUniqueCount,
        int difficulty,
        double averageNumberOfDigits,
        int candidateCount)
    {
        if (difficulty >= 4)
        {
            return candidateUniqueCount
                 * difficulty
                 * (candidateCount - 1)
                 * averageNumberOfDigits;
        }

        return candidateUniqueCount
             * difficulty
             * averageNumberOfDigits;
    }
}
