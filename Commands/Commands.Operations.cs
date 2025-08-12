public static partial class Commands
{
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

    public static void Delete(string[] args)
    {
        if (args.Length < 3 || args.Length > 4)
        {
            Console.WriteLine("usage: del [-f / -d] [filename / foldername] [opt. -p]");
            return;
        }

        string option = args[1];
        string name = args[2];
        bool permanent = args.Length == 4 && args[3] == "-p";
        string path = Path.Combine(Globals.currentDir, name);

        if (permanent)
        {
            string itemType = option == "-f" ? "file" : option == "-d" ? "directory" : "item";

            Console.WriteLine($"{itemType} will get permanently deleted");
            Console.Write("continue? (y/n): ");
            var input = Console.ReadLine();

            if (input?.ToLower() != "y")
            {
                Console.WriteLine("cancelled");
                return;
            }

            try
            {
                if (option == "-f")
                {
                    if (File.Exists(path))
                    {

                        File.Delete(path);
                        Console.WriteLine($"permanently deleted file: {name}");

                    }
                    else Console.WriteLine("file does not exist");
                }
                else if (option == "-d")
                {
                    if (Directory.Exists(path))
                    {

                        Directory.Delete(path, true);
                        Console.WriteLine($"permanently deleted directory: {name}");

                    }
                    else Console.WriteLine("directory does not exist");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"error permanently deleting: {e.Message}");
            }
        }
        else
        {
            string trashDir = Path.Combine(Globals.homeDir, ".trash");
            Directory.CreateDirectory(trashDir);

            string dest = Path.Combine(trashDir, name);

            try
            {
                if (option == "-f")
                {
                    if (File.Exists(path))
                    {

                        File.Move(path, dest);
                        Console.WriteLine($"moved file to trash: {name}");

                    }
                    else Console.WriteLine("file does not exist");
                }
                else if (option == "-d")
                {
                    if (Directory.Exists(path))
                    {

                        Directory.Move(path, dest);
                        Console.WriteLine($"moved directory to trash: {name}");

                    }
                    else Console.WriteLine("directory does not exist");
                }
                else
                {
                    Console.WriteLine("unknown option");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"error moving to trash: {e.Message}");
            }
        }
    }

}