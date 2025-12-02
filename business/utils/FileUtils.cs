using System.IO;

namespace business.utils;

public static class FileUtils
{
    /// <summary>
    /// 获取目录下所有文件。
    /// </summary>
    public static IEnumerable<string> GetFiles(string dir)
        => Directory.GetFiles(dir);

    /// <summary>
    /// 获取目录下指定扩展名的文件（如 ".csv"）。
    /// </summary>
    public static IEnumerable<string> GetFiles(string dir, string extension)
        => Directory.GetFiles(dir, $"*{extension}");

    /// <summary>
    /// 获取目录下多个扩展名的文件。
    /// </summary>
    public static IEnumerable<string> GetFiles(string dir, params string[] extensions)
    {
        foreach (var ext in extensions)
        foreach (var file in Directory.GetFiles(dir, $"*{ext}"))
            yield return file;
    }

    /// <summary>
    /// 递归获取整个目录树的所有文件。
    /// </summary>
    public static IEnumerable<string> GetAllFiles(string dir)
        => Directory.EnumerateFiles(dir, "*", SearchOption.AllDirectories);
}
