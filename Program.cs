using System.Diagnostics;

class Program
{
    static void Main()
    {
        Globals.currentDir = "C:\\";
        while (true)
        {
            Console.Write($"\nBindershell | [\u001b[32m{Globals.currentDir}\u001b[0m] > ");
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
            case "mem":
                MeasureTime(() => Commands.Mem(parts));
                break;

            // File Organization
            case "find":
                MeasureTime(() => Commands.Find(parts));
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
                MeasureTime(() => Console.WriteLine($"total uptime: {Globals.upTime.Elapsed.ToString(@"hh\:mm\:ss")}"));
                break;
            case "clear":
                Commands.Clear();
                break;
            case "history":
                int index = 1;
                foreach (var entry in Globals.commandHistory)
                {
                    Console.WriteLine($"{index++}: {entry}");
                }
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
        Console.WriteLine($"\u001b[35mtime taken: {stopWatch.Elapsed.TotalMilliseconds} ms\u001b[0m");
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