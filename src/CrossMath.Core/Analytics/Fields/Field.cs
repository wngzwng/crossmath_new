namespace CrossMath.Core.Analytics.Fields;
public interface IField
{
    string Name { get; }
    Type FieldType { get; }
}

public abstract record FieldBase(string Name, Type FieldType) : IField;

public sealed record Field<T>(string Name)
    : FieldBase(Name, typeof(T));
    