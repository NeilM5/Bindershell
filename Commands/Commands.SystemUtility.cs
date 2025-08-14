using System.Runtime.InteropServices;
using System.Text.Json;

public static partial class Commands
{
    public static void Help()
    {
        Console.WriteLine(@"
        usage: [command] [options] [arguments] [extra options]
        key: opt. = optional

        Directory and Navigation Commands
        ---------------------------------
        - cd [path]                                             change directory
        - list                                                  list all directories and files
        - tree                                                  show a visual tree of folders and files (with warning)

        File or Folder Operations
        ---------------
        - create [-f / -d] [filename / foldername]              creates a file or folder
        - move [-f / -d] [filename /foldername] [path]          moves a file or folder to a new location
        - copy [-f / -d] [filename / foldername] [opt. path]    copies a file or folder to a new location
        - rename [old] [new]                                    renames a file or folder to the new name
        - del [-f / -d] [filename / foldername] [opt. -p]       deletes a file or folder (-p for permanent)

        File Organization and Information
        -------------------------------
        - sort [check sort options below]                       list files organized by type, size, and date created
        - find [extension]                                      lists path to files with provided extension
        - mem [filename]                                        list the memory usage of a file
        
        Utility and Customization
        -------------------------
        - help                                                  show list of available commands
        - ver [opt. -ls]                                        show Bindershell version (-ls will list all available versions)
        - mode [shell / box]                                    switch between Bindershell and Binderbox mode
        - clear                                                 clears console screen
        - time                                                  displays current system time
        - uptime                                                displays total time Bindershell is in use
        - history                                               lists all commands used so far
        - info                                                  prints Bindershell and system info (with logo)
        - theme [-ls / -#]                                      lists themes (-ls) or changes it by theme number (-#)
        - exit                                                  exits Bindershell (also displays final uptime)

        (deprecated) BinderBox Commands
        ------------------
        - bbox add [filename]                                   add a file to binderbox
        - bbox extract [filename]                               extract a file from binderbox to current directory
        - bbox list                                             list all files curently stored in binderbox

        Sort Options
        ------------
        - sort -f -t                                            sort files by type (extension)
        - sort [-f / -d] -s                                     sort files or directories by size
        - sort [-f / -d] -dc                                    sort files or directories by date created
        ");
    }

    public static void Ver(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine(Globals.version);
            return;
        }

        string option = args[1];

        if (option == "-ls")
        {
            GetVersions().GetAwaiter().GetResult();
        }
    }

    private static async Task GetVersions()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("Bindershell-Versions");

        string repo = "NeilM5/Bindershell";
        string url = $"https://api.github.com/repos/{repo}/tags";

        var response = await client.GetStringAsync(url);

        using var doc = JsonDocument.Parse(response);
        var tags = doc.RootElement.EnumerateArray();

        Console.WriteLine("available versions:");
        foreach (var tag in tags)
        {
            string version = tag.GetProperty("name").GetString() ?? "unknown version";
            Console.WriteLine($"- {version}");
        }
    }

    public static void Mode(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("usage: mode [shell / box]");
            return;
        }

        string mode = args[1];

        if (mode != "shell" && mode != "box")
        {
            Console.WriteLine($"invalid mode: {mode}; available modes: shell, box");
            return;
        }

        Globals.currentMode = mode;
        Console.WriteLine($"mode changed to: {Globals.currentMode}");
    }

    public static void CurrentTime()
    {
        Console.WriteLine(DateTime.Now.ToString("MMM d yyyy | ddd | h:mm:ss tt"));
    }

    public static void UpTime()
    {
        Console.WriteLine($"total uptime: {Globals.upTime.Elapsed.ToString(@"hh\:mm\:ss")}");
    }

    public static void Clear()
    {
        Console.Clear();
    }

    public static void History()
    {
        int index = 1;
        foreach (var entry in Globals.commandHistory)
        {
            Console.WriteLine($"{index++}: {entry}");
        }
    }

    public static void BinderBoxCommand(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("usage: [command] [args]; use 'help' to view available Binderbox commands");
            return;
        }

        string cmd = args[0].ToLower();

        switch (cmd)
        {
            case "help":
                BinderBox.Help();
                break;
            case "add":
                if (args.Length >= 2)
                {
                    string fullPath = Path.Combine(Globals.currentDir, args[1]);
                    BinderBox.Add(fullPath);
                }
                else Console.WriteLine("usage: add [filename]");
                break;
            case "extract":
                if (args.Length >= 2)
                {
                    string fileName = args[1];
                    BinderBox.Extract(fileName);
                }
                else Console.WriteLine("usage: extract [filename]");
                break;
            case "list":
                BinderBox.List();
                break;
            case "mem":
                BinderBox.Mem();
                break;
            default:
                Console.WriteLine($"invalid Binderbox command: {cmd}; use 'help' to see available commands");
                break;
        }
    }

    public static void Info()
    {
        string color = Themes.GetColor("dirColor");
        string end = "\u001b[0m";

        Console.WriteLine($@"

                    ****************                {color}Bindershell {Globals.version}{end}
                 **********************             {color}OS: {end}{RuntimeInformation.OSDescription}
              ********            ********          {color}User: {end}{Environment.UserName}
            *******                  *******        {color}Machine: {end}{Environment.MachineName}
           ******                      ******       {color}Uptime: {end}{Globals.upTime.Elapsed.ToString(@"hh\:mm\:ss")}
          ******                        ******      {color}Current Directory: {end}{Globals.currentDir}
          ******       {color}**********{end}       ******      {color}Current Mode: {end}{Globals.currentMode}
          ******     {color}**************{end}     ******      {color}Theme: {end}{Themes.GetThemeName()}
          ******    {color}***          ***{end}    ******
           ******  {color}***            ***{end} *******
            ******{color}***              ***{end}******
              *********          *********
                 **********************
                    ****************
        ");
    }

    public static void ChangeTheme(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("usage: theme [-ls / -#]");
            return;
        }

        string option = args[1];

        if (option == "-ls")
        {
            int index = 1;
            foreach (var name in Themes.themes.Keys)
            {
                Console.WriteLine($"[{index++}] {name}");
            }
        }

        try
        {
            switch (option)
            {
                case "-1":
                    Themes.SetTheme("Default");
                    Console.WriteLine("new theme set: Default");
                    break;
                case "-2":
                    Themes.SetTheme("Ocean");
                    Console.WriteLine("new theme set: Ocean");
                    break;
                case "-3":
                    Themes.SetTheme("Sunset");
                    Console.WriteLine("new theme set: Sunset");
                    break;
            }
        }
        catch
        {
            Console.WriteLine($"theme does not exist");
        }
    }
}