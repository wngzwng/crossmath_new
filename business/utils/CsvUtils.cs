using System.Globalization;
using business.Csv;
using CsvHelper;

namespace business.utils;

public static class CsvUtils
{
    public static IEnumerable<T> Read<T>(string file) where T : class
    {
        Console.WriteLine("\n Read From: " + file);
        using var reader = new StreamReader(file);
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        // 注册所有 Map
        CsvProfile.Register(csv.Context);

        foreach (var record in csv.GetRecords<T>())
        {
            yield return record;   // ⭐ 在 using 内枚举 → 安全！
        }
    }

    public static void Write<T>(string file, IEnumerable<T> data)
    {
        using var writer = new StreamWriter(file);
        using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

        // 注册所有 Map
        CsvProfile.Register(csv.Context);

        csv.WriteRecords(data);
        Console.WriteLine("\n Saved To: " + file);
    }
}
