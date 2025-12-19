using System.CommandLine;
using CrossMath.Core.Utils.Progress;
using CrossMath.Service.Jobs;

namespace CrossMath.CLI;

public static class Program
{
    public static async Task<int> Main(string[] args)
    {
        var root = new RootCommand("CrossMath CLI 工具集");
        // 注册子命令
        root.Add(Framework.Docs.DocsCommand.Build(root));
        root.Add(new Commands.BartestCommand().Build());
        root.Add(new Commands.HoleCommand().Build());
        root.Add(new Commands.ConvertCommand().Build());
        root.Add(new Commands.InitAnalyticsCommand().Build());
        root.Add(new DiagramDirective());
        return await root.Parse(args).InvokeAsync();
    }
}

// var inputfile = "/Users/admin/RiderProjects/Puzzle/CrossMath/data/merged_distinct.csv";
// var finalOutput = "/Users/admin/RiderProjects/Puzzle/CrossMath/data/merged_distinct_new_encode.csv";
//
// var job = new ConvertJob(new StdOutProgressWriter());
// job.Run(inputfile, finalOutput);

/**
find  /Users/admin/RiderProjects/Puzzle/CrossMath/data/split -name '*.csv' -print0 \
| parallel -0 -j 8 \
    --joblog hole.log \
    --halt soon,fail=1 \
    ./CrossMath.CLI  hole -i {} --output-dir /Users/admin/RiderProjects/Puzzle/CrossMath/data/split_result


find /Users/admin/RiderProjects/Puzzle/CrossMath/data/merged_randomed_split -name '*.csv' -print0 \
| time parallel -0 -j 3 \
    --bar \
    --eta \
    --joblog analytics_init.log \
    ./CrossMath.CLI  analytics-init \
        -i {} \
        --output-dir /Users/admin/RiderProjects/Puzzle/CrossMath/data/merged_randomed_split_result \
        -p --run-count 1000


 ./CrossMath.CLI  analytics-init \
        -i /Users/admin/RiderProjects/Puzzle/CrossMath/data/merged_randomed_split/merged_randomed_part0001.csv \
        --output-dir /Users/admin/RiderProjects/Puzzle/CrossMath/data/merged_randomed_split_result \
        -p --run-count 1000
        
        
 find /Users/admin/RiderProjects/Puzzle/CrossMath/data/merged_randomed_split -name '*.csv' -print0 \
| time parallel -0 -j 3 \
--ungroup   --joblog analytics_init.log \ 
    ./CrossMath.CLI  analytics-init \
        -i {} \
        --output-dir /Users/admin/RiderProjects/Puzzle/CrossMath/data/merged_randomed_split_result \
        -p --run-count 1000
*/