using business.works.Layout;
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
        double maxSigma = 6.0)
    {
        return new LayoutGenerationJob
        {
            CanvasSize = size,
            MinFormulaCount = minFormulaCount,
            MaxFormulaCount = maxFormulaCount,

            MaxSigma = maxSigma,

            PlacementGenerator = new PlacementGenerator()
                .WithPlaceStrategies([(5, CrossType.Number), (7, CrossType.Number)])
                .StopAtFirstMatch(false),

            InitPlacements = InitialPlacementGenerator.BuildPlacement([
                (size, 5, CrossType.Number),
                (size, 7, CrossType.Number),
            ]),

            TargetCount = targetCount
        };
    }
    
    
    public static IEnumerable<LayoutGenerationJob> CreateJobsFromSpecs(
        int targetCount = 30000,
        double maxSigma = 6.0,
        IStopPolicy? stopPolicy = null
        )
    {
        foreach (var (size, spec) in LayoutFormulaSpec.Specs)
        {
            IPlacementGenerator placementGenerator = new PlacementGenerator()
                .WithPlaceStrategies([(5, CrossType.Number), (7, CrossType.Number)])
                .StopAtFirstMatch(false);
            
            yield return new LayoutGenerationJob
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
                
                StopPolicy = stopPolicy,
            };
        }
    }
}
