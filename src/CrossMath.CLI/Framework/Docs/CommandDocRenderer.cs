using System.CommandLine;
using System.Text;

namespace CrossMath.CLI.Framework.Docs;

public class CommandDocRenderer
{
    // === Markdown 模板（可随意修改）==========================
    private const string Template = """
# `{Name}` — {Description}

## 📘 描述
{Description}

## 🧭 用法（Usage）
```bash
{Usage}
```
{OptionsSection}
{ArgumentsSection}
{SubcommandsSection}
""";
    
    public static string Render(Command cmd)
    {
        var context = new Dictionary<string, string>
        {
            ["Name"] = cmd.Name,
            ["Description"] = cmd.Description ?? "",
            ["Usage"] = GetUsage(cmd),
            ["OptionsSection"] = BuildOptionsSection(cmd.Options),
            ["ArgumentsSection"] = BuildArgumentsSection(cmd.Arguments),
            ["SubcommandsSection"] = BuildSubcommandsSection(cmd.Subcommands)
        };

        return RenderTemplate(Template, context);
    }

    // === 文本模板渲染 ========================================
    private static string RenderTemplate(string template, Dictionary<string, string> ctx)
    {
        foreach (var (key, value) in ctx)
            template = template.Replace($"{{{key}}}", value ?? "");
        return template;
    }

    // === Usage ==============================================
    private static string GetUsage(Command cmd)
    {
        var sb = new StringBuilder();
        sb.Append(cmd.Name);

        foreach (var o in cmd.Options)
            sb.Append($" [--{o.Name}]");

        foreach (var a in cmd.Arguments)
            sb.Append($" <{a.Name}>");

        return sb.ToString();
    }

    // === Options ============================================
    private static string BuildOptionsSection(IEnumerable<Option> options)
    {
        if (!options.Any())
            return "## 📝 选项（Options）\n\n_此命令没有选项_\n";

        var sb = new StringBuilder();
        sb.AppendLine("## 📝 选项（Options）\n");
        sb.AppendLine("| 名称 | 类型 | 描述 | 默认值 |");
        sb.AppendLine("|------|------|------|---------|");

        foreach (var o in options)
        {
            var type = o.ValueType?.Name ?? "";
            var desc = o.Description ?? "";
            var def = o.GetDefaultValue()?.ToString() ?? "";
            sb.AppendLine($"| --{o.Name} | {type} | {desc} | {def} |");
        }

        sb.AppendLine();
        return sb.ToString();
    }

    // === Arguments ==========================================
    private static string BuildArgumentsSection(IEnumerable<Argument> args)
    {
        if (!args.Any())
            return "## 📌 参数（Arguments）\n\n_此命令没有参数_\n";

        var sb = new StringBuilder();
        sb.AppendLine("## 📌 参数（Arguments）\n");

        foreach (var a in args)
        {
            sb.AppendLine($"### `{a.Name}`");
            sb.AppendLine($"类型：`{a.ValueType.Name}`\n");
            sb.AppendLine(a.Description ?? "");
            sb.AppendLine();
        }

        return sb.ToString();
    }

    // === Subcommands =========================================
    private static string BuildSubcommandsSection(IEnumerable<Command> subs)
    {
        if (!subs.Any())
            return "## 📂 子命令（Subcommands）\n\n_此命令没有子命令_\n";

        var sb = new StringBuilder();
        sb.AppendLine("## 📂 子命令（Subcommands）\n");

        foreach (var s in subs)
            sb.AppendLine($"- **{s.Name}** — {s.Description}");

        sb.AppendLine();
        return sb.ToString();
    }
}
