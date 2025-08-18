using System.Diagnostics;
using System.Runtime.InteropServices;
public static class Globals
{
    public static string currentDir = Directory.GetCurrentDirectory();
    public static string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    public static string version = "v1.14.3";
    public static string currentMode = "shell";
    public static List<string> commandHistory = new();
    public static Stopwatch upTime = Stopwatch.StartNew();

    public static string[] winForbiddenPaths = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? new[]
    {
        @"C:\",
        @"C:\Windows",
        @"C:\Program Files",
        @"C:\Program Files (x86)",
        @"C:\Users",
        @"C:\PerfLogs"
    } : Array.Empty<string>();

    public static string[] linuxForbiddenPaths = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? new[]
    {
        "/",
        "/boot",
        "/dev",
        "/etc",
        "/lib",
        "/lib64",
        "/proc",
        "/sys",
        "/usr",
        "/var",
        "/home",
        "/mnt"
    } : Array.Empty<string>();

    public static string[] macForbiddenPaths = RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? new[]
    {
        "/",
        "/System",
        "/bin",
        "/usr",
        "/Library",
        "/Users"
    } : Array.Empty<string>();

    public static string[] ForbiddenPaths => winForbiddenPaths.Concat(linuxForbiddenPaths).Concat(macForbiddenPaths).ToArray();
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