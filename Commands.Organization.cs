public static partial class Commands
{
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

}