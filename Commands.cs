public static class Commands
{
    public static void Help()
    {
        Console.WriteLine(@"
        usage: [command] [options] [arguments]

        Directory and Navigation Commands
        ---------------------------------
        - cd [path]                                        change directory
        - list                                             list all directories and files
        - tree                                             show a visual tree of folders and files (with warning)

        File or Folder Operations
        ---------------
        - create [-f / -d] [filename / foldername]         creates a file or folder
        - move [-f / -d] [filename /foldername] [path]     moves a file or folder to a new location
        - copy [-f / -d] [filename / foldername] [path]    copies a file or folder to a new location (or current directory if no path arg)
        - rename [old] [new]                               renames a file or folder to the new name
        - del [-f / -d] [filename / foldername]            deletes a file or folder (with warning)

        File Organization and Information
        -------------------------------
        - sort [check sort options below]                  list files organized by type, size, and date created
        - find [extension]                                 lists path to files with provided extension
        - mem [filename]                                   list the memory usage of a file
        
        Utility and Customization
        -------------------------
        - help                                             show list of available commands
        - ver                                              show Bindershell version
        - clear                                            clears console screen
        - time                                             displays current system time
        - uptime                                           displays total time Bindershell is in use
        - history                                          lists all commands used so far
        - exit                                             exits Bindershell (also displays final uptime)

        BinderBox Commands
        ------------------
        - bbox add [filename]                              add a file to binderbox
        - bbox extract [filename]                          extract a file from binderbox to current directory
        - bbox list                                        list all files curently stored in binderbox

        Sort Options
        ------------
        - sort -f -t                                       sort files by type (extension)
        - sort [-f / -d] -s                                sort files or directories by size
        - sort [-f / -d] -dc                               sort files or directories by date created
        ");
    }
    public static void CurrentTime()
    {
        Console.WriteLine(DateTime.Now.ToString("MMM d yyyy | ddd | h:mm:ss tt"));
    }

    public static void ChangeDirectory(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("usage: cd [path]");
            return;
        }

        string inputPath = args[1];
        string path = Path.IsPathRooted(inputPath) ? inputPath : Path.Combine(Globals.currentDir, inputPath);

        if (Directory.Exists(path))
        {
            Globals.currentDir = Path.GetFullPath(path);
            Console.WriteLine($"changed directory to: {Globals.currentDir}");
        }
        else Console.WriteLine("directory not found");
    }

    public static void List()
    {
        string[] dirs = Directory.GetDirectories(Globals.currentDir);
        string[] files = Directory.GetFiles(Globals.currentDir);

        foreach (var dir in dirs)
        {
            Console.WriteLine($"[dir] {Path.GetFileName(dir)}");
        }

        foreach (var file in files)
        {
            Console.WriteLine($"[file] {Path.GetFileName(file)}");
        }
    }

    public static void Tree(string path, string indent = "")
    {
        if (!Directory.Exists(path))
        {
            Console.WriteLine("directory does not exist");
            return;
        }

        int count = 0;
        void Estimate(string p, int depth)
        {
            try
            {
                count += Directory.GetFiles(p).Length;
                if (depth <= 0) return;

                foreach (var dir in Directory.GetDirectories(p))
                {
                    count++;
                    Estimate(dir, depth - 1);
                }
            }
            catch (UnauthorizedAccessException) { }
        }

        Estimate(path, 2);

        if (count > 500)
        {
            Console.WriteLine($"warning: {path} has over {count} items; this may take a while");
            Console.Write("continue? (y/n): ");
            var input = Console.ReadLine();
            if (input?.ToLower() != "y")
            {
                Console.WriteLine("cancelled");
                return;
            }
        }

        Traverse(path, indent);
    }

    private static void Traverse(string path, string indent, bool isLast = true)
    {
        try
        {
            var files = Directory.GetFiles(path);
            var dirs = Directory.GetDirectories(path);

            var entries = dirs.Concat(files).ToList();

            for (int i = 0; i < entries.Count; i++)
            {
                var isDir = Directory.Exists(entries[i]);
                var isLastEntry = i == entries.Count - 1;

                string connector = isLastEntry ? "└── " : "├── ";
                Console.WriteLine($"{indent}{connector}{Path.GetFileName(entries[i])}");

                if (isDir)
                {
                    string newIndent = indent + (isLastEntry ? "    " : "│   ");
                    Traverse(entries[i], newIndent, isLastEntry);
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            Console.WriteLine($"└── {indent}[access denied]");
        }
    }

    public static void Create(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("usage: create [-f / -d] [filename / foldername]");
            return;
        }

        string option = args[1];
        string name = args[2];
        string path = Path.Combine(Globals.currentDir, name);

        if (option == "-f")
        {
            if (File.Exists(path))
            {
                Console.WriteLine("file already exists");
                return;
            }

            try
            {
                File.Create(path).Dispose();
                Console.WriteLine($"created file: {name}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"error creating file: {e.Message}");
            }
        }
        else if (option == "-d")
        {
            if (Directory.Exists(path))
            {
                Console.WriteLine("directory already exists");
                return;
            }

            try
            {
                Directory.CreateDirectory(path);
                Console.WriteLine($"created directory: {name}");
            }
            catch (Exception e)
            {
                Console.WriteLine($"error creating directory: {e.Message}");
            }
        }
        else Console.WriteLine($"unknown option: {option}; use -f or -d");
    }

    public static void Delete(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("usage: del [-f / -d] [filename / foldername]");
            return;
        }

        string option = args[1];
        string name = args[2];
        string path = Path.Combine(Globals.currentDir, name);

        if (option == "-f")
        {
            try
            {
                if (File.Exists(path))
                {
                    Console.WriteLine("file will get permanently deleted");
                    Console.Write("continue? (y/n): ");
                    var input = Console.ReadLine();

                    if (input?.ToLower() == "y")
                    {
                        File.Delete(path);
                        Console.WriteLine($"deleted file: {name}");
                    }
                    else Console.WriteLine("cancelled");
                }
                else Console.WriteLine("file does not exist");
            }
            catch (Exception e)
            {
                Console.WriteLine($"error deleting file: {e.Message}");
            }
        }
        else if (option == "-d")
        {
            try
            {
                if (Directory.Exists(path))
                {
                    Console.WriteLine("folder and its contents will get permanently deleted");
                    Console.Write("continue? (y/n): ");
                    var input = Console.ReadLine();

                    if (input?.ToLower() == "y")
                    {
                        Directory.Delete(path, true);
                        Console.WriteLine($"deleted directory: {name}");
                    }
                    else Console.WriteLine("cancelled");
                }
                else Console.WriteLine("directory does not exist");
            }
            catch (Exception e)
            {
                Console.WriteLine($"error deleting folder: {e.Message}");
            }
        }
        else Console.WriteLine($"unknown option: {option}; use -f or -d");
    }

    public static void Move(string[] args)
    {
        if (args.Length < 4)
        {
            Console.WriteLine("usage: move [-f / -d] [filename / foldername] [path]");
            return;
        }

        string option = args[1];
        string name = args[2];
        string src = Path.Combine(Globals.currentDir, name);
        string dst = Path.IsPathRooted(args[3]) ? args[3] : Path.Combine(Globals.currentDir, args[3]);

        if (option == "-f")
        {
            try
            {
                if (File.Exists(src))
                {
                    if (Directory.Exists(dst))
                    {
                        dst = Path.Combine(dst, Path.GetFileName(src));
                    }

                    File.Copy(src, dst);
                    Console.WriteLine($"copied file to {dst}: {name}");
                }
                else Console.WriteLine("file does not exist");
            }
            catch (Exception e)
            {
                Console.WriteLine($"error: {e.Message}");
            }
        }

        else if (option == "-d")
        {
            try
            {
                if (Directory.Exists(src))
                {
                    if (Directory.Exists(dst))
                    {
                        dst = Path.Combine(dst, Path.GetFileName(src));
                    }

                    //Directory.(src, dst);
                    Console.WriteLine($"moved directory to {dst}: {name}");
                }
                else Console.WriteLine("directory does not exist");
            }
            catch (Exception e)
            {
                Console.WriteLine($"error: {e.Message}");
            }
        }

        else Console.WriteLine($"unknown option: {option}; use -f or -d");
    }

    public static void Copy(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("usage: copy [-f / -d] [filename / foldername] [optional: path]");
            return;
        }

        string option = args[1];
        string name = args[2];
        string current = Globals.currentDir;
        string src = Path.Combine(current, name);

        bool isFile = option == "-f";
        if ((isFile && !File.Exists(src)) || (!isFile && !Directory.Exists(src)))
        {
            Console.WriteLine(isFile ? "file does not exist" : "directory does not exist");
            return;
        }

        string dst;
        bool duplicate = args.Length < 4;

        if (duplicate)
        {
            dst = GetUniquePath(src, isFile);
        }
        else
        {
            string rawDst = Path.IsPathRooted(args[3]) ? args[3] : Path.Combine(current, args[3]);

            if (Directory.Exists(rawDst))
            {
                dst = Path.Combine(rawDst, Path.GetFileName(src));
            }
            else dst = rawDst;

            string? dstFolder = isFile ? Path.GetDirectoryName(dst) : dst;

            if (dstFolder != null && (Path.GetFullPath(dstFolder).TrimEnd(Path.DirectorySeparatorChar) == Path.GetFullPath(current).TrimEnd(Path.DirectorySeparatorChar)))
            {
                dst = GetUniquePath(dst, isFile);
            }
        }

        try
        {
            if (isFile)
            {
                File.Copy(src, dst);
                Console.WriteLine($"copied file to: {dst}");
            }
            else
            {
                CopyDirectory(src, dst, true);
                Console.WriteLine($"copied directory to: {dst}");
            }

        }
        catch (Exception e)
        {
            Console.WriteLine($"error: {e.Message}");
        }
    }

    private static string GetUniquePath(string basePath, bool isFile)
    {
        string? dir = Path.GetDirectoryName(basePath);
        string name = isFile ? Path.GetFileNameWithoutExtension(basePath) : Path.GetFileName(basePath);
        string ext = isFile ? Path.GetExtension(basePath) : "";

        int i = 1;

        string temp = basePath;

        do
        {
            if (dir == null)
            {
                Console.WriteLine("cannot determine directory for path");
                return "";
            }
            
            temp = Path.Combine(dir, $"{name}_{i++}{ext}");
        }
        while ((isFile && File.Exists(temp)) || (!isFile && Directory.Exists(temp)));

        return temp;
    }

    private static void CopyDirectory(string srcDir, string dstDir, bool canCopySubDirs)
    {
        Directory.CreateDirectory(dstDir);

        foreach (var file in Directory.GetFiles(srcDir))
        {
            File.Copy(file, Path.Combine(dstDir, Path.GetFileName(file)));
        }

        if (canCopySubDirs)
        {
            foreach (var subDir in Directory.GetDirectories(srcDir))
            {
                CopyDirectory(subDir, Path.Combine(dstDir, Path.GetFileName(subDir)), true);
            }
        }
    }

    public static void Rename(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("usage: rename [old] [new]");
            return;
        }

        string oldName = args[1];
        string newName = args[2];

        string src = Path.Combine(Globals.currentDir, oldName);
        string dst = Path.Combine(Globals.currentDir, newName);

        try
        {
            if (File.Exists(src))
            {
                File.Move(src, dst);
                Console.WriteLine($"renamed file: {oldName} -> {newName}");
            }
            else if (Directory.Exists(src))
            {
                Directory.Move(src, dst);
                Console.WriteLine($"renamed directory: {oldName} -> {newName}");
            }
            else Console.WriteLine("file or directory does not exist");
        }
        catch (Exception e)
        {
            Console.WriteLine($"error: {e.Message}");
        }
    }

    public static void Mem(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("usage: mem [filename]");
            return;
        }

        string path = Path.Combine(Globals.currentDir, args[1]);

        if (File.Exists(path))
        {
            long fileSize = new FileInfo(path).Length;
            Console.WriteLine($"{args[1]} - {FormatBytes(fileSize)}");
        }
        else Console.WriteLine("file not found");
    }

    public static string FormatBytes(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;

        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len /= 1024;
        }

        return $"{len:0.##} {sizes[order]}";
    }

    public static void Find(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("usage: find [extension]");
            return;
        }

        var extension = args[1];

        if (!extension.StartsWith(".")) extension = "." + extension;

        try
        {
            var files = Directory.EnumerateFiles(Globals.currentDir, "*" + extension, SearchOption.AllDirectories);
            int count = 0;
            foreach (var file in files)
            {
                string relPath = Path.GetRelativePath(Globals.currentDir, file);
                count++;
                Console.WriteLine(relPath);
            }
            Console.WriteLine($"\nfound {count} file(s) with extension {extension}");

            if (!files.Any()) Console.WriteLine($"no {extension} files found");
        }
        catch (Exception e)
        {
            Console.WriteLine($"error finding files: {e.Message}");
        }
    }

    public static void Sort(string[] args)
    {
        if (args.Length < 3)
        {
            Console.WriteLine("usage: sort [-f / -d] [-t / -s / -dc]");
        }

        string option = args[1];
        string sortFlag = args[2];

        if (option == "-f")
        {
            var files = Directory.GetFiles(Globals.currentDir);
            IEnumerable<string> sorted = Enumerable.Empty<string>();

            switch (sortFlag)
            {
                case "-t":
                    sorted = files.OrderBy(f => Path.GetExtension(f));
                    break;
                case "-s":
                    sorted = files.OrderBy(f => new FileInfo(f).Length);
                    break;
                case "-dc":
                    sorted = files.OrderBy(f => File.GetCreationTime(f));
                    break;
                default:
                    Console.WriteLine($"unknown sort option: {sortFlag}");
                    break;
            }

            if (sorted == null) return;

            foreach (var file in sorted)
            {
                var fileInfo = new FileInfo(file);

                switch (sortFlag)
                {
                    case "-t":
                        Console.WriteLine($"[.{fileInfo.Extension.TrimStart('.')}] {fileInfo.Name,-30}");
                        break;
                    case "-s":
                        Console.WriteLine($"[{FormatBytes(fileInfo.Length),10}] {fileInfo.Name,-30}");
                        break;
                    case "-dc":
                        Console.WriteLine($"[{fileInfo.CreationTime}] {fileInfo.Name,-30}");
                        break;
                }
            }
        }

        else if (option == "-d")
        {
            var dirs = Directory.GetDirectories(Globals.currentDir);
            IEnumerable<string> sorted = Enumerable.Empty<string>();

            switch (sortFlag)
            {
                case "-s":
                    sorted = dirs.OrderBy(d => GetDirectorySize(d));
                    break;
                case "-dc":
                    sorted = dirs.OrderBy(d => Directory.GetCreationTime(d));
                    break;
                default:
                    Console.WriteLine($"unknown sort option: {sortFlag}");
                    break;
            }

            if (sorted == null) return;

            foreach (var dir in sorted)
            {
                var name = Path.GetFileName(dir);

                switch (sortFlag)
                {
                    case "-s":
                        long size = GetDirectorySize(dir);
                        Console.WriteLine($"[{FormatBytes(size),10}] {name,-30}");
                        break;
                    case "-dc":
                        Console.WriteLine($"[{Directory.GetCreationTime(dir)}] {name,-30}");
                        break;
                }
            }
        }

        else Console.WriteLine($"unknown option: {option}; use -f or -d");
    }

    private static long GetDirectorySize(string path)
    {
        try
        {
            if (!Directory.Exists(path))
            {
                return 0;
            }

            long size = 0;

            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);

            foreach (string file in files)
            {
                try
                {
                    FileInfo info = new FileInfo(file);
                    size += info.Length;
                }
                catch { }
            }

            return size;
        }
        catch (Exception e)
        {
            Console.WriteLine($"error: {e.Message}");
            return 0;
        }
    }

    public static void Clear()
    {
        Console.Clear();
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