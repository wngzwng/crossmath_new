using CrossMath.Core.Analytics.Fields;
namespace CrossMath.Core.Analytics.Schemas;

public sealed class Schema
{
    public string Name { get; }
    public IReadOnlyList<IField> Fields { get; }

    public Schema(string name, IEnumerable<IField> fields)
    {
        Name = name;
        Fields = fields.ToList();
    }
}
