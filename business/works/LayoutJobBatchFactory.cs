using business.works.Layout;
using CrossMath.Core.Expressions.Layout;
using CrossMath.Core.Generators.Collectors;
using CrossMath.Core.Generators.CompletionCheckers;
using CrossMath.Core.Generators.ExpandControllers;
using CrossMath.Core.Types;
using CrossMath.Core.Generators.PlacementGenerators; 
using CrossMath.Core.Generators.StopPolicies;

namespace business.works;

public static class LayoutJobBatchFactory
{
    public static LayoutGenerationJob CreateJobForSize(
        Size size,
        int minFormulaCount,
        int maxFormulaCount,
        int targetCount = 30000,
        double maxSigma = 6.0,
        IStopPolicy? stopPolicy= null,
        ICompletionChecker? completionChecker = null)
    {
        return new LayoutGenerationJob
        {
            CanvasSize = size,
            MinFormulaCount = minFormulaCount,
            MaxFormulaCount = maxFormulaCount,

            MaxSigma = maxSigma,

            PlacementGenerator = new PlacementGenerator()
                .WithPlaceStrategies([
                    (7, CrossType.Number),
                    (5, CrossType.Number), 
                ])
                .StopAtFirstMatch(false),

            InitPlacements = InitialPlacementGenerator.BuildPlacement([
                (size, 5, CrossType.Number),
                (size, 7, CrossType.Number),
            ]),

            TargetCount = targetCount,
            
            StopPolicy = stopPolicy,
                
            CompletionChecker = completionChecker
        };
    }
    
    
    public static IEnumerable<LayoutGenerationJob> CreateJobsFromSpecs(
        int targetCount = 30000,
        double maxSigma = 6.0,
        Func<LayoutGenerationJob, BucketCounter<int>?, IStopPolicy>? stopPolicyFactory = null,
        Func<LayoutGenerationJob, BucketCounter<int>?, ICompletionChecker>? completionCheckerFactory = null,
        Func<LayoutGenerationJob, BucketCounter<int>?, IExpandController>? expandControllerFactory = null,
        Func<int, BucketCounter<int>>? bucketCounterFactory = null)
    {
        foreach (var (size, spec) in LayoutFormulaSpec.Specs)
        {
            // var placementGenerator = new WeightedSevenPlacementGenerator();
            var placementGenerator = new PlacementGenerator()
                .WithPlaceStrategies([
                    (5, CrossType.Number),
                    (7, CrossType.Number),
                ]).StopAtFirstMatch(false);
            //
            // 先创建 counter（这里用的是全局的 targetCount，和你原来的意图一致）
            var counter = bucketCounterFactory?.Invoke(targetCount);

            var job = new LayoutGenerationJob
            {
                CanvasSize = size,
                MinFormulaCount = spec.Min,
                MaxFormulaCount = spec.Max,
                MaxSigma = maxSigma,
                PlacementGenerator = placementGenerator,
                InitPlacements = InitialPlacementGenerator.BuildPlacement([
                    (size, 5, CrossType.Number),
                    (size, 7, CrossType.Number),
                ]),
                TargetCount = targetCount,

                // 直接赋值
                counter = counter,
            };

            job.ExpandController = expandControllerFactory?.Invoke(job, counter);
            // 现在 counter 已经就位，再创建依赖它的策略
            job.StopPolicy = stopPolicyFactory?.Invoke(job, counter);
            job.CompletionChecker = completionCheckerFactory?.Invoke(job, counter);

            yield return job;
        }
    }
}
