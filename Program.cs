using System.Diagnostics;
using System.Runtime.InteropServices;

class Program
{
    static void Main()
    {
        Globals.currentDir = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "C:\\" : "/";
        while (true)
        {
            Console.Write($"\nBindershell | [{Themes.GetColor("dirColor")}{Globals.currentDir}{Themes.GetColor("end")}] > ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input)) continue;
            else Globals.commandHistory.Add(input);

            if (input.Trim().ToLower() == "exit")
            {
                Globals.upTime.Stop();
                Console.WriteLine($"total uptime: {Globals.upTime.Elapsed.ToString(@"hh\:mm\:ss")}");
                break;
            }

            try
            {
                Run(input.Trim());
            }
            catch (Exception e)
            {
                Console.WriteLine($"error: {e.Message}");
            }
        }
    }

    static void Run(string input)
    {
        var parts = SplitArgs(input);
        if (parts.Length == 0) return;

        string cmd = parts[0].ToLower();

        switch (cmd)
        {
            // Dictionary & Navigation
            case "cd":
                MeasureTime(() => Commands.ChangeDirectory(parts));
                break;
            case "list":
                MeasureTime(Commands.List);
                break;
            case "tree":
                MeasureTime(() => Commands.Tree(Globals.currentDir));
                break;

            // File Operations
            case "create":
                MeasureTime(() => Commands.Create(parts));
                break;
            case "del":
                MeasureTime(() => Commands.Delete(parts));
                break;
            case "move":
                MeasureTime(() => Commands.Move(parts));
                break;
            case "copy":
                MeasureTime(() => Commands.Copy(parts));
                break;
            case "rename":
                MeasureTime(() => Commands.Rename(parts));
                break;

            // File Organization & Information
            case "sort":
                MeasureTime(() => Commands.Sort(parts));
                break;
            case "find":
                MeasureTime(() => Commands.Find(parts));
                break;
            case "mem":
                MeasureTime(() => Commands.Mem(parts));
                break;

            // Utility & Customization
            case "help":
                Commands.Help();
                break;
            case "ver":
                Console.WriteLine(Globals.version);
                break;
            case "time":
                MeasureTime(Commands.CurrentTime);
                break;
            case "uptime":
                MeasureTime(Commands.UpTime);
                break;
            case "clear":
                Commands.Clear();
                break;
            case "history":
                MeasureTime(Commands.History);
                break;
            case "info":
                Commands.Info();
                break;
            case "theme":
                MeasureTime(() => Commands.ChangeTheme(parts));
                break;

            // BinderBox
            case "bbox":
                MeasureTime(() => Commands.BinderBoxCommand(parts));
                break;
                
            default:
                Console.WriteLine($"invalid command: {cmd}");
                break;
        }
    }

    static void MeasureTime(Action action)
    {
        var stopWatch = Stopwatch.StartNew();

        try
        {
            action();
        }
        catch (Exception e)
        {
            Console.WriteLine($"error: {e.Message}");
        }

        stopWatch.Stop();
        Console.WriteLine($"{Themes.GetColor("timeColor")}time taken: {stopWatch.Elapsed.TotalMilliseconds} ms{Themes.GetColor("end")}");
    }

    static string[] SplitArgs(string input)
    {
        var inputList = new List<string>();
        var current = "";
        bool inQuotes = false;

        foreach (char c in input)
        {
            if (c == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }

            if (char.IsWhiteSpace(c) && !inQuotes)
            {
                if (current.Length > 0)
                {
                    inputList.Add(current);
                    current = "";
                }
            }

            else
            {
                current += c;
            }
        }

        if (current.Length > 0)
        {
            inputList.Add(current);
        }

        return inputList.ToArray();
    }
}