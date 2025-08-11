public static partial class Commands
{
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

}