namespace business.Csv;

public class CsvConfig
{
    // 默认数据目录
    public static string RootDirectory { get; private set; }
        = Path.Combine(ProjectRoot, "Data");
    
    public static void SetDataDirectory(string path)
    {
        RootDirectory = path;
    }

    public static string ResolvePath(string fileName)
    {
        var dir = RootDirectory;
        Directory.CreateDirectory(dir);
        return Path.Combine(dir, fileName);
    }
    
    public static string ProjectRoot
    {
        get
        {
            var dir = AppContext.BaseDirectory;

            // bin/Debug/net8.0 → 回退到项目根目录
            return Directory.GetParent(dir)   // net8.0
                       ?.Parent                     // Debug
                       ?.Parent                     // bin
                       ?.Parent                     // 项目根目录
                       ?.FullName 
                   ?? dir;
        }
    }
}