using CrossMath.Core.Codec;
using CrossMath.Core.Models;
using CrossMath.Core.Utils.Progress;
using CrossMath.Service.Pipelines;
using CrossMath.Service.Utils;

namespace CrossMath.Service.Jobs;

public class InitBoardAnalysisJob
{
    private readonly BoardAnalysisPipeline _pipeline;
    private int _levelRunCount;
    private readonly IProgressWriter? _progressWriter;
    public InitBoardAnalysisJob(
        BoardAnalysisPipeline pipeline,
        int levelRunCount,
        IProgressWriter? progressWriter = null)
    {
        _pipeline = pipeline;
        _levelRunCount = levelRunCount;
        
        _progressWriter = progressWriter;
    }

    public void Run(string inputFile, string outputFile)
    {
        var filledBoards = CsvUtils.ReadDictCsv(inputFile).ToList();
        var results = new List<OrderedDictionary<string, object?>>();
        var progress = _progressWriter != null
            ? new Tqdm(filledBoards.Count, $"分析初盘中 ${Path.GetFileNameWithoutExtension(inputFile)}",
                writer: _progressWriter)
            : null;
        
        for (int i = 0; i < filledBoards.Count; i++)
        {
            var record = filledBoards[i];
            var board = Decode(record!);

            var result = _pipeline.Run(board, _levelRunCount);
            results.Add(Merge(record, result));
            
            progress?.Update();
        }

        CsvUtils.WriteDictCsv(outputFile, results);
    }

    private BoardData Decode(IDictionary<string, object> record)
    {
        var layoutStr = (string)record["layout_info"]!;
        var level = (string)record["start_info"]!;

        var board = BoardDataCodec.Decode(level, layoutStr);
        return board;
    }

    private OrderedDictionary<string, object?> Merge(OrderedDictionary<string, object?> record, BoardAnalysis result)
    {
        											
        var merged = new OrderedDictionary<string, object?>(record);
        merged["difficulty"] = string.Join("", result.GlobalDifficulties);
        merged["min_diff"] = result.GlobalDifficulties.Min();
        merged["max_diff"] = result.GlobalDifficulties.Max();
        merged["total_diff"] = result.GlobalDifficulties.Sum();
        
        merged["start_diff"] = string.Join("", result.LocalDifficulties);
        
        merged["stuck_num"] = result.StuckCount;
        merged["stucks_point"] = result.StuckPoints;
        merged["first_stuck_point"] = result.FirstStuckPoint;
        merged["first_stuck_point_percent"] = Math.Round(result.FirstStuckPointPercent, 3);
        merged["empty_friendly"] = Math.Round(result.EmptyFriendly, 3);
        merged["max_weight_coordinates"] = string.Join(",",
            result.MaxWeightPath
                .Select(v => $"({v.row}, {v.col}, {v.localDifficulty})"));
        merged["max_weight_score"] = Math.Round(result.MaxWeightLevelScore, 3);
        merged["rand_score"] = Math.Round(result.LevelScore, 3);
        return merged;;
    }
}