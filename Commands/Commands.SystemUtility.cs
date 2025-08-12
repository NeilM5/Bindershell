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
        - ver                                                   show Bindershell version
        - clear                                                 clears console screen
        - time                                                  displays current system time
        - uptime                                                displays total time Bindershell is in use
        - history                                               lists all commands used so far
        - exit                                                  exits Bindershell (also displays final uptime)

        BinderBox Commands
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
        if (args.Length < 2)
        {
            Console.WriteLine("usage: bbox [command] [arg]");
        }

        string cmd = args[1];

        switch (cmd)
        {
            case "add":
                if (args.Length >= 3)
                {
                    string fullPath = Path.Combine(Globals.currentDir, args[2]);
                    BinderBox.Add(fullPath);
                }
                else Console.WriteLine("usage: bbox add [filename]");
                break;
            case "extract":
                if (args.Length >= 3)
                {
                    string fileName = args[2];
                    BinderBox.Extract(fileName);
                }
                else Console.WriteLine("usage: bbox extract [filename]");
                break;
            case "list":
                BinderBox.List();
                break;
            case "mem":
                BinderBox.Mem();
                break;
        }
    }
}