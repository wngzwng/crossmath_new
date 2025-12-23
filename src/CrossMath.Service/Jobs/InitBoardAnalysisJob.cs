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
            BoardAnalysis result;
            try
            {
                result = _pipeline.Run(board, _levelRunCount);
                results.Add(Merge(record, result));
            }
            catch (InvalidOperationException)
            {
                // 预期失败：跳过
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
            finally
            {
                progress?.Update();
            }
            
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
        merged["stucks_point"] = string.Join(",",result.StuckPoints);
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

/*
1,7726fb08fb04fa32fbfbfc031bfc0dfa0efbfafbfa05232524fafa2efb04fa32,1111111101000110111111010101101010110001001111100,7x7,7,2,2,1.66,8,0.88,0,7,5,7,5,1.0,1,1,0,1,3,50,1.33,"7,2,0,0","4,0,0,0",1,0,0,
7726fb00fb00fa00fbfbfc031bfc00fa0efbfafbfa00000024fafa00fb00fa00:0804320d0523252e0432,"8,4,50,13,5,35,37,46,4,50",
10,
2211222222,
1,
2,
18
,2311223243,
2,
3,
5,
3,
0.2,
1.6,
"(1, 7, 1),(3, 5, 1),(5, 5, 2),(7, 5, 1),(7, 1, 2),(5, 1, 1),(7, 3, 1),(5, 3, 2),(1, 3, 1),(1, 5, 1)",
100.499,
108.517
   

*/