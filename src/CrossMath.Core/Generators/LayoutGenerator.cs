using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Generators.PlacementGenerators;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Generators;

public class LayoutGenerator
{
    private PlacementGenerator placementGenerators;
    private List<(int, CrossType)> placeStrategies;
    
    public LayoutGenerator(PlacementGenerator placementGenerator)
    {
        this.placementGenerators = placementGenerator;
    }

    public BoardLayout? Generator(ICanvas canvas, IEnumerable<(int, CrossType)> placeStrategies)
    {
        this.placeStrategies = placeStrategies.ToList();
        return GeneratorCore(canvas, 0);
    }
    public BoardLayout? GeneratorCore(ICanvas canvas, int depth = 0)
    {
        var boundingBoxSize = canvas.GetBoundingBoxSize();
        // if (depth > 10)  
        if (boundingBoxSize.Width >= 10 && boundingBoxSize.Height >= 10)
            return canvas.ExportBoardLayout();

        foreach (var placement in GeneratorPlaces(canvas, placeStrategies))
        {
            var backup = canvas.Clone();

            backup.TryApplyPlacement(placement, out _);
            
            backup.ExportBoardLayout().PrettyPrint();
            
            var result = GeneratorCore(backup, depth + 1);
            if (result != null)
                return result;
        }

        return canvas.ExportBoardLayout();
    }


    IEnumerable<Placement> GeneratorPlaces(ICanvas canvas, IEnumerable<(int, CrossType)> placeStrategies)
    {
        foreach (var (placementLength, crossType) in placeStrategies)
        {
            var placementIter = placementGenerators.Generate(canvas, placementLength, crossType);
            if (placementIter.TryUncons(out var head, out var tail))
            {
                yield return head;     // 第一个候选
                foreach (var x in tail)
                    yield return x;    // 继续
                yield break;
            }
        }
    }
    
    

}