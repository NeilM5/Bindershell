public static partial class Commands
{
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
    
}