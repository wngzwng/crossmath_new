using CrossMath.Core.Codec;
using CrossMath.Core.Models;
using CrossMath.Service.Utils.Progress;
using CrossMath.Service.Pipelines;
using CrossMath.Service.Utils;

namespace CrossMath.Service.Jobs;

public sealed class HoleJob
{
    private readonly HoleBoardPipeline _pipeline;
    private readonly IProgressWriter? _progressWriter;

    public HoleJob(
        HoleBoardPipeline pipeline,
        IProgressWriter? progressWriter = null)
    {
        _pipeline = pipeline;
        _progressWriter = progressWriter;
    }

    public void Run(string inputFile, string outputFile)
    {
        var filledBoards = CsvUtils.ReadDictCsv(inputFile).ToList();
        var results = new List<OrderedDictionary<string, object?>>();
        var progress = _progressWriter != null
            ? new Tqdm(filledBoards.Count, $"挖空中 ${Path.GetFileNameWithoutExtension(inputFile)}",
                writer: _progressWriter)
            : null;
        Console.WriteLine($"count: {filledBoards.Count}");
        for (int i = 0; i < filledBoards.Count; i++)
        {
            var record = filledBoards[i];
            var board = Decode(record);

            foreach (var result in _pipeline.Run(board))
            {
                results.Add(Merge(record, result));
            }

            progress?.Update();
        }

        CsvUtils.WriteDictCsv(outputFile, results);
    }

    private BoardData Decode(IDictionary<string, object> record)
    {
        var layoutStr = (string)record["layout_info"]!;
        var level = (string)record["end_info"]!;

        var board = BoardDataCodec.Decode(level, layoutStr);
        return board;
    }

    private OrderedDictionary<string, object?> Merge(OrderedDictionary<string, object?> record, InitBoardResult result)
    {
        var merged = new OrderedDictionary<string, object?>(record);
        merged["start_info"] = result.StartInfo;
        merged["answer"] = string.Join(",",result.Answers);
        merged["cell_num_empty"] = result.EmptyCellCount;
        return merged;;
    }
}
