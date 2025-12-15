using business.Records;
using business.utils;
using CrossMath.Core.Analytics.FinalBoard;
using CrossMath.Core.ExpressionSolvers;
using CrossMath.Core.ExpressionSolvers.ExpressionValidators;
using CrossMath.Core.ExpressionSolvers.NumberPools;
using CrossMath.Core.ExpressionSolvers.OperatorPools;
using CrossMath.Core.ExpressionSolvers.SolverProviders;
using CrossMath.Core.Fillers;
using CrossMath.Core.Models;
using CrossMath.Core.Types;
using CrossMath.Core.Utils;

namespace business.Scripts;

public static class Scripts_12_08
{
    // 填盘器
    private static readonly LayoutFiller s_layoutFiller = 
        new LayoutFiller(ExpressionSolverProvider.CreateDefault());

    // 随机参数
    private static readonly WeightedRandom<int> s_minNumberWeightRandomer 
        = WeightedRandom.Create((1, 7), (2, 3));

    private static readonly WeightedRandom<int> s_maxNumberWeightRandomer 
        = WeightedRandom.Create([(10, 1), (20, 1), (30, 1), (50, 1), (80, 1), (120, 1)]);

    private static readonly WeightedRandom<IOperatorPool> s_operatorWeightRandomer 
        = WeightedRandom.Create([
            (OperatorPoolFactory.AS, 2),
            (OperatorPoolFactory.ASM, 1),
            (OperatorPoolFactory.MDAS, 1),
        ]);

    // 终盘分析器
    private static readonly FinalBoardAnalyzer finalBoardAnalyzer = new FinalBoardAnalyzer();


    // =====================================================
    // 主流程入口：可直接用于批处理
    // =====================================================
    public static void Run(string inputFile, string outputFile, int? countLimit = null)
    {
        Console.WriteLine("=== Generate Final Boards ===");

        // ============================================================
        // 1. 读取空盘分析结果（CSV → Dictionary）
        // ============================================================
        var emptyBoards = CsvUtils.Read<LayoutFullRecord>(inputFile)
            .Select((record, i) =>
            {
                record.Id = i + 1;
                return record;
            }).ToList();
        if (countLimit.HasValue)
            emptyBoards = emptyBoards.Where(b => b.Id <= countLimit.Value).ToList();
        
        Console.WriteLine($"Loaded Empty Boards: {emptyBoards.Count}");

        // 最终输出的数据
        var finalRecords = new List<OrderedDictionary<string, object?>>();

        // ============================================================
        // 处理每一个空盘
        // ============================================================
        var bar = new TqdmProgressPrinter($"fill board {Path.GetFileNameWithoutExtension(inputFile)}");
        var curIndex = 0;
        bar.Report(curIndex, emptyBoards.Count);
        foreach (var empty in emptyBoards)
        {
            var layoutStr = empty.LayoutInfo;
            var size = empty.Size;

            var boardLayout = new BoardLayout(layoutStr, size.Width, size.Height);

            // 每个空盘生成多个终盘（例如 5 个）
            int[] maxValueGroup = [10, 20, 30, 50, 80, 120];
            int i = 0;
            const int maxtryCount = 5;
            int failedCount = 0;
            while (i < maxValueGroup.Length)
            {
                var solverCtx = CreateSolverContext(maxValueGroup[i]);
                // 尝试生成终盘
                if (!TryFill(boardLayout, solverCtx, 1000, 3, out var board)) 
                {
                    failedCount++;
                    if (failedCount > maxtryCount)
                    {
                        failedCount = 0;
                        i++;
                    }
                    continue;
                }

                failedCount = 0;

                // ====================================================
                // 3. 终盘分析
                // ====================================================
                var finalInfo = finalBoardAnalyzer.Analyze(board);

                // ====================================================
                // 4. 初盘 + 终盘分析结果 → 合并成字典
                // ====================================================
                var finalDict = new OrderedDictionary<string, object?>();

                // 初盘信息加入字典
                foreach (var (k, v) in empty.ToDict())
                    finalDict[k] = v;
                
                // 终盘分析字典
                foreach (var (k, v) in finalInfo.ToDict())
                    finalDict[k] = v;
                
                // 保存结果
                finalRecords.Add(finalDict);
                i++;
            }
            
            bar.Report(++curIndex, emptyBoards.Count);
        }

        // ============================================================
        // 输出最终 CSV
        // ============================================================
        CsvUtils.WriteDictCsv(
            outputFile,
            finalRecords
        );

        Console.WriteLine($"Saved {finalRecords.Count} final boards to final_boards.csv");
        Console.WriteLine("=== Finish ===");
    }



    // =====================================================
    // 填盘（带重试次数）
    // singMaxAttempts: 单一填法最大尝试次数
    // mulMaxAttempts: 外层整体重试次数
    // =====================================================
    public static bool TryFill(
        BoardLayout boardLayout,
        ExpressionSolveContext solveContext,
        int singMaxAttempts,
        int mulMaxAttempts,
        out BoardData board)
    {
        board = null;
        s_layoutFiller.Setup(solveContext);

        while (mulMaxAttempts-- > 0)
        {
            if (s_layoutFiller.TryFill(boardLayout, singMaxAttempts, [5, 7], out board, out var successIndex))
            {
                Console.WriteLine($"成功填盘（successIndex = {successIndex}）");
                return true;
            }
            else
            {
                Console.WriteLine($"填盘失败: ");
                boardLayout.LogicPrettyPrint();
            }
        }

        return false;
    }


    // =====================================================
    // 求解上下文随机化
    // =====================================================
    public static ExpressionSolveContext CreateSolverContext(int? maxValue = null,
        int maxMulDivByOne = 2, int maxSameExpression = 2)
    {
        var minNumber = s_minNumberWeightRandomer.Next();
        var maxNumber = maxValue.HasValue ? maxValue.Value : s_maxNumberWeightRandomer.Next();

        var validator = CompositeExpressionValidator.And(
            new ExpressionValidator(ValidationMode.FullPoolCheck),
            new MulDivByOneLimitValidator(maxMulDivByOne),
            new SameExpressionLimitValidator(maxSameExpression)
        );

        var opPool = s_operatorWeightRandomer.Next();

        Console.WriteLine($"生成求解上下文：min={minNumber}, max={maxNumber}, opPool={opPool}");

        return new ExpressionSolveContext()
        {
            NumPool = NumberPoolFactory.Create(minNumber, maxNumber, NumberOrder.Shuffled),
            OpPool = opPool,
            Validator = validator
        };
    }
}
