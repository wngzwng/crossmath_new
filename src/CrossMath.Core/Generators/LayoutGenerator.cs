using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Generators.PlacementGenerators;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Generators;

public class LayoutGenerator
{
    private PlacementGenerator placementGenerators;
    private int placementLength;
    private CrossType crossType;
    
    public LayoutGenerator(PlacementGenerator placementGenerator)
    {
        this.placementGenerators = placementGenerator;
    }

    public BoardLayout? Generator(ICanvas canvas, int placementLength, CrossType crossType)
    {
        this.crossType = crossType;
        this.placementLength = placementLength;
        return GeneratorCore(canvas, 0);
    }
    public BoardLayout? GeneratorCore(ICanvas canvas, int depth = 0)
    {
        var boundingBoxSize = canvas.GetBoundingBoxSize();
        // if (depth > 4)  
        if (boundingBoxSize.Width >= 9 && boundingBoxSize.Height >= 9)
            return canvas.ExportBoardLayout();

        foreach (var placement in GeneratorPlaces(canvas, [
                     (7, CrossType.Operator), 
                     (7, CrossType.Number), 
                     (5, CrossType.Operator), 
                     (5, CrossType.Number)]))
        {
            var backup = canvas.Clone();

            canvas.TryApplyPlacement(placement, out _);
            
            canvas.ExportBoardLayout().PrettyPrint();
            
            var result = GeneratorCore(canvas, depth + 1);
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