using CrossMath.Core.Codec;
using CrossMath.Core.Models;
using CrossMath.Core.Utils.Progress;
using CrossMath.Service.Utils;

namespace CrossMath.Service.Jobs;

public class ConvertJob
{
    private readonly IProgressWriter? _progressWriter;

    public ConvertJob(
        IProgressWriter? progressWriter = null)
    {
        _progressWriter = progressWriter;
    }

    public void Run(string inputFile, string outputFile)
    {
        var boardInfos = CsvUtils.ReadDictCsv(inputFile).ToList();
        
        var progress = _progressWriter != null
            ? new Tqdm(boardInfos.Count, $"转换中 ${Path.GetFileNameWithoutExtension(inputFile)}",
                writer: _progressWriter)
            : null;
        Console.WriteLine($"count: {boardInfos.Count}");
        for (int i = 0; i < boardInfos.Count; i++)
        {
            var record = boardInfos[i];
            var board = Decode(record!);
            
            var (encoded, _) = BoardDataCodec.Encode(board);
            record["start_info"] = encoded;
            
            progress?.Update();
        }

        CsvUtils.WriteDictCsv(outputFile, boardInfos);
    }

    private BoardData Decode(IDictionary<string, object> record)
    {
        var layoutStr = (string)record["layout_info"]!;
        var level = (string)record["start_info"]!;

        var board = BoardDataCodec.Decode(level, layoutStr);
        return board;
    }
    
}