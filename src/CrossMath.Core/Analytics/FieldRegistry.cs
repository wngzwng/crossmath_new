using CrossMath.Core.Models;
using CrossMath.Core.Analytics.Fields;

namespace CrossMath.Core.Analytics;

public sealed class FieldRegistry
{
    private readonly Dictionary<IField, Func<BoardLayout, BoardData?, object?>> _computers = new();

    public void Register<T>(
        Field<T> field,
        Func<BoardLayout, BoardData?, T> computer)
    {
        if (_computers.ContainsKey(field))
            throw new InvalidOperationException(
                $"Field '{field.Name}' already registered");

        _computers[field] = (layout, board) =>
        {
            var value = computer(layout, board);
            if (value is not T)
                throw new InvalidOperationException(
                    $"Computer for field '{field.Name}' returned wrong type");
            return value;
        };
    }

    public Func<BoardLayout, BoardData?, object?> Get(IField field)
    {
        if (_computers.TryGetValue(field, out var c))
            return c;

        throw new InvalidOperationException(
            $"No computer registered for field '{field.Name}'");
    }
    
    
}

