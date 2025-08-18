using System.Runtime.InteropServices;

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

    public static void Link(string[] args)
    {
        if (args.Length < 4)
        {
            Console.WriteLine("usage: link [-s] [target] [linkname]");
            return;
        }

        string option = args[1];
        string target = Path.IsPathRooted(args[2]) ? args[2] : Path.Combine(Globals.currentDir, args[2]);
        string linkname = Path.IsPathRooted(args[3]) ? args[3] : Path.Combine(Globals.currentDir, args[3]);

        try
        {
            if (option == "-s")
            {
                if (!File.Exists(target) && !Directory.Exists(target))
                {
                    Console.WriteLine("file or directory does not exist");
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (!IsWindowsDevModeEnabled())
                    {
                        Console.WriteLine("warning: Windows Developer Mode is not enabled; symlinks may require admin privileges");
                    }
                }
                
                if (File.Exists(target))
                {
                    File.CreateSymbolicLink(linkname, target);
                    Console.WriteLine($"file symbolic link created: {linkname} -> {target}");
                }
                else if (Directory.Exists(target))
                {
                    Directory.CreateSymbolicLink(linkname, target);
                    Console.WriteLine($"directory symbolic link created: {linkname} -> {target}");
                }
            }
            else Console.WriteLine($"unknown option: {option}; use -h or -s");
        }
        catch (Exception e)
        {
            Console.WriteLine($"error: {e.Message}");
        }
    }

    private static bool IsWindowsDevModeEnabled()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return true;

        try
        {
            using var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\AppModelUnlock",
                false
            );

            if (key != null)
            {
                var value = key.GetValue("AllowDevelopmentWithoutDevLicense");
                return value != null && (int)value == 1;
            }
        }
        catch { }

        return false;
    }
}