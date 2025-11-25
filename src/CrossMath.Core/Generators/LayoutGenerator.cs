using CrossMath.Core.Generators.Canvas;
using CrossMath.Core.Generators.PlacementGenerators;
using CrossMath.Core.Models;
using CrossMath.Core.Types;

namespace CrossMath.Core.Generators;

public class LayoutGenerator
{
    private PlacementGenerator placementGenerators;

    public LayoutGenerator(PlacementGenerator placementGenerator)
    {
        this.placementGenerators = placementGenerator;
    }
    public BoardLayout? Generator(ICanvas canvas)
    {
        return GeneratorCore(canvas, 0);
    }
    
    public BoardLayout? GeneratorCore(ICanvas canvas, int depth = 0)
    {
        var boundingBoxSize = canvas.GetBoundingBoxSize();
        // if (depth > 10)  
        if (boundingBoxSize.Width >= 10 && boundingBoxSize.Height >= 10)
            return canvas.ExportBoardLayout();

        foreach (var placement in placementGenerators.Generate(canvas))
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

}