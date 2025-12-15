using CrossMath.Core.Models;
using CrossMath.Core.Analytics.Fields;
namespace CrossMath.Core.Analytics;

public sealed class FieldComputeRunner
{
    private readonly FieldRegistry _registry;

    public FieldComputeRunner(FieldRegistry registry)
    {
        _registry = registry;
    }

    public Dictionary<IField, object?> Run(
        IEnumerable<IField> fields,
        BoardLayout layout,
        BoardData? boardData = null)
    {
        var result = new Dictionary<IField, object?>();

        foreach (var field in fields)
        {
            var computer = _registry.Get(field);
            result[field] = computer(layout, boardData);
        }

        return result;
    }
}
