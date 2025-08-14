using System.Diagnostics;
public static class Globals
{
    public static string currentDir = Directory.GetCurrentDirectory();
    public static string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    public static string version = "v1.8.6";
    public static string currentMode = "shell";
    public static List<string> commandHistory = new();
    public static Stopwatch upTime = Stopwatch.StartNew();
}

public static class Themes
{
    public static readonly Dictionary<string, Dictionary<string, string>> themes = new()
    {
        ["Default"] = new Dictionary<string, string>
        {
            ["dirColor"] = "\u001b[38;2;111;194;118m",
            ["timeColor"] = "\u001b[38;2;204;55;147m",
            ["cmdColor"] = "\u001b[38;2;255;243;109m",
            ["end"] = "\u001b[0m"
        },

        ["Ocean"] = new Dictionary<string, string>
        {
            ["dirColor"] = "\u001b[38;2;16;156;197m",
            ["timeColor"] = "\u001b[38;2;60;178;235m",
            ["cmdColor"] = "\u001b[38;2;56;200;216m",
            ["end"] = "\u001b[0m"
        },

        ["Sunset"] = new Dictionary<string, string>
        {
            ["dirColor"] = "\u001b[38;2;251;144;98m",
            ["timeColor"] = "\u001b[38;2;238;93;108m",
            ["cmdColor"] = "\u001b[38;2;255;193;107m",
            ["end"] = "\u001b[0m"
        }
    };

    public static Dictionary<string, string> currentTheme = themes["Default"];

    public static string GetColor(string theme)
    {
        if (currentTheme.TryGetValue(theme, out var color))
        {
            return color;
        }

        return currentTheme["end"];
    }

    public static bool SetTheme(string themeName)
    {
        if (themes.TryGetValue(themeName, out var theme))
        {
            currentTheme = theme;
            return true;
        }
        return false;
    }

    public static string GetThemeName()
    {
        foreach (var kvp in themes)
        {
            if (kvp.Value == currentTheme) return kvp.Key;
        }

        return "Unknown";
    }
}