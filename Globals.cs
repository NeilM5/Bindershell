using System.Diagnostics;
public static class Globals
{
    public static string currentDir = Directory.GetCurrentDirectory();
    public static string version = "v1.0.0";

    public static List<string> commandHistory = new();

    public static Stopwatch upTime = Stopwatch.StartNew();
}