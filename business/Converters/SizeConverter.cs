using CrossMath.Core.Types;
using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace business.Converters;

public class SizeConverter : ITypeConverter
{
    private readonly char _separator;

    public SizeConverter(char separator = 'x')
    {
        _separator = separator;
    }

        
    public static SizeConverter Create(char separator = 'x')
    {
        return new SizeConverter(separator);
    }
    
    public object ConvertFromString(string text, IReaderRow row, MemberMapData data)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new Size(0, 0);

        var parts = text.Split(_separator);

        if (parts.Length != 2)
            throw new FormatException($"Size 格式错误: \"{text}\"（需要格式: width{_separator}height）");

        int height = int.Parse(parts[0]);
        int width = int.Parse(parts[1]);

        return new Size(width, height);
    }

    public string ConvertToString(object value, IWriterRow row, MemberMapData data)
    {
        var size = (Size)value;
        return $"{size.Height}{_separator}{size.Width}";
    }
}