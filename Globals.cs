using System.Diagnostics;
public static class Globals
{
    public static string currentDir = Directory.GetCurrentDirectory();
    public static string homeDir = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    public static string version = "v1.6.5";

    public static List<string> commandHistory = new();

    public static Stopwatch upTime = Stopwatch.StartNew();
}