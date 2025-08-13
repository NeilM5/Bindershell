using System.Diagnostics;
public static class Globals
{
    public static string currentDir = Directory.GetCurrentDirectory();
    public static string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    public static string version = "v1.6.5";

    public static List<string> commandHistory = new();

    public static Stopwatch upTime = Stopwatch.StartNew();
}

public static class Themes
{
    public static readonly Dictionary<string, Dictionary<string, string>> themes = new()
    {
        ["Default"] = new Dictionary<string, string>
        {
            ["dirColor"] = "\u001b[32m",
            ["timeColor"] = "\u001b[32m",
            ["end"] = "\u001b[0m"
        },

        ["Ocean"] = new Dictionary<string, string>
        {
            ["dirColor"] = "\u001b[34m",
            ["timeColor"] = "\u001b[36m",
            ["end"] = "\u001b[0m"
        },

        ["Sunset"] = new Dictionary<string, string>
        {
            ["dirColor"] = "\u001b[38;5;214m",
            ["timeColor"] = "\u001b[38;5;220m",
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