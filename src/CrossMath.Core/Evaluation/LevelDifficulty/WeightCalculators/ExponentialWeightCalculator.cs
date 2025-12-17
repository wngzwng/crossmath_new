using CrossMath.Core.Types;
namespace CrossMath.Core.Evaluation.LevelDifficulty.WeightCalculators;

/// <summary>
/// 指数衰减权重计算器
///
/// 根据 score 计算权重：
///
///     weight = exp(- score / temperature)
///
/// 该实现直接对应 Python 中的：
///
///     e ** (-score / 0.8)
///
/// 语义说明：
/// - score 越大，权重越小（更不容易被选中）
/// - temperature 控制衰减速度：
///   - 小 → 更偏向最大 score（贪婪）
///   - 大 → 分布更平滑（随机性更强）
///
/// 注意：
/// - 本计算器【不做归一化】
/// - 是否归一化 / 如何使用权重，由 SelectionPolicy 决定
/// </summary>
public sealed class ExponentialWeightCalculator : IWeightCalculator
{
    /// <summary>
    /// 温度参数（temperature）
    /// 对应 Python 中 score / 0.8 的分母
    /// </summary>
    private readonly double _temperature;

    /// <summary>
    /// 创建一个指数权重计算器
    /// </summary>
    /// <param name="temperature">
    /// 温度参数，必须大于 0；
    /// 值越小，权重分布越“尖锐”
    /// </param>
    public ExponentialWeightCalculator(double temperature = 0.8)
    {
        if (temperature <= 0)
            throw new ArgumentOutOfRangeException(
                nameof(temperature),
                "Temperature must be positive");

        _temperature = temperature;
    }

    /// <summary>
    /// 根据 score 计算每个候选坐标的权重
    /// </summary>
    /// <param name="context">
    /// LevelDifficulty 上下文（当前实现未使用，但保留以支持上下文相关权重）
    /// </param>
    /// <param name="scores">
    /// 每个候选坐标的评分（score）
    /// </param>
    /// <returns>
    /// 每个候选坐标对应的权重（未归一化）
    /// </returns>
    public IReadOnlyDictionary<RowCol, double> Calculate(
        LevelDifficultyContext context,
        IReadOnlyDictionary<RowCol, double> scores)
    {
        var result = new Dictionary<RowCol, double>(scores.Count);

        foreach (var (coord, score) in scores)
        {
            // weight = e^(-score / temperature)
            double weight = Math.Exp(-score / _temperature);
            result[coord] = weight;
        }

        return result;
    }
}