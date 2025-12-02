using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace business.Converters;

public class IntArrayConverter : ITypeConverter
{
    private readonly char _separator;

    public IntArrayConverter(char separator = ',')
    {
        _separator = separator;
    }
    
        
    public static IntArrayConverter Create(char separator = ',')
    {
        return new IntArrayConverter(separator);
    }
    
    public object ConvertFromString(string text, IReaderRow row, MemberMapData data)
    {
        return text.Split(_separator).Select(int.Parse).ToArray();
    }

    public string ConvertToString(object value, IWriterRow row, MemberMapData data)
    {
        var arr = (int[])value;
        return string.Join(_separator, arr);
    }
}
