using LiteDB;

public static class BinderBox
{
    private static readonly string binderFile = Path.Combine(AppContext.BaseDirectory, "binderbox.bbox");

    public static void Help()
    {
        Console.WriteLine(@"
        Binderbox
        ---------

        usage: [command] [args]

        - help                                  lists the available Binderbox commands
        - add [filename/foldername]             adds a file to Binderbox from the current directory
        - extract [filename/foldername]         extracts a file from Binderbox into the current directory
        - list                                  lists any files (path if folder) stored in Binderbox
        - mem                                   prints the total size of Binderbox
        - clear                                 permanently deletes all files and folders in Binderbox (with warning)

        - mode [shell / box]                    switch between Bindershell or Binderbox mode
        ");
    }

    public static void Add(string path)
    {
        path = Path.GetFullPath(path);

        if (!File.Exists(path) && !Directory.Exists(path))
        {
            Console.WriteLine("file or folder not found in binderbox");
            return;
        }

        foreach (var forbidden in Globals.ForbiddenPaths)
        {
            if (path.Equals(forbidden, StringComparison.OrdinalIgnoreCase) ||
            path.StartsWith(forbidden + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine("access denied: cannot add this file or folder into Binderbox");
                return;
            }
        }


        using var db = new LiteDatabase(binderFile);
        var storage = db.GetStorage<string>();

        if (File.Exists(path))
        {
            string fileName = Path.GetFileName(path);

            storage.Delete(fileName);
            storage.Upload(fileName, path);
            File.Delete(path);

            Console.WriteLine($"added file to Binderbox: {fileName}");
        }
        else if (Directory.Exists(path))
        {
            string root = Path.GetFullPath(path);
            string folderName = Path.GetFileName(root);
            var allFiles = Directory.GetFiles(root, "*", SearchOption.AllDirectories);

            if (allFiles.Length > 100)
            {
                Console.WriteLine($"warning: folder contains more than {allFiles.Length} files; this may take a while");
                Console.Write("continue? (y/n): ");
                var input = Console.ReadLine();

                if (input?.ToLower() != "y")
                {
                    Console.WriteLine("cancelled");
                    return;
                }
            }

            foreach (var file in allFiles)
            {
                string relInsideFolder = Path.GetRelativePath(root, file).Replace(Path.DirectorySeparatorChar, '/');
                string storagePath = folderName + "/" + relInsideFolder;
                storage.Delete(storagePath);
                storage.Upload(storagePath, file);
                File.Delete(file);
            }

            Directory.Delete(root, true);

            Console.WriteLine($"added folder to Binderbox: {path} ({allFiles.Length} files)");
        }

    }

    public static void Extract(string path)
    {
        using var db = new LiteDatabase(binderFile);
        var storage = db.GetStorage<string>();

        if (storage.Exists(path))
        {
            // Single File
            string outPath = Path.Combine(Globals.currentDir, path.Replace('/', Path.DirectorySeparatorChar));
            string outDir = Path.GetDirectoryName(outPath)!;
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }

            using var outFile = File.Create(outPath);
            storage.Download(path, outFile);
            storage.Delete(path);

            Console.WriteLine($"extracted file from Binderbox: {path}");
            return;
        }

        var files = storage.FindAll()
                    .Where(f => f.Id.StartsWith(path.TrimEnd('/') + "/", StringComparison.OrdinalIgnoreCase))
                    .ToList();

        if (files.Count == 0)
        {
            Console.WriteLine("file or folder not found in Binderbox");
            return;
        }

        foreach (var file in files)
        {
            string outPath = Path.Combine(Globals.currentDir, file.Id.Replace('/', Path.DirectorySeparatorChar));
            string outDir = Path.GetDirectoryName(outPath)!;
            if (!Directory.Exists(outDir))
            {
                Directory.CreateDirectory(outDir);
            }

            using var outFile = File.Create(outPath);
            storage.Download(file.Id, outFile);
            storage.Delete(file.Id);
        }

        Console.WriteLine($"extracted folder from Binderbox: {path}");
    }

    public static void List()
    {
        using var db = new LiteDatabase(binderFile);
        var storage = db.GetStorage<string>();

        var allFiles = storage.FindAll();

        bool empty = true;
        foreach (var fileInfo in allFiles)
        {
            empty = false;
            Console.WriteLine($"{fileInfo.Id} | size: {Commands.FormatBytes(fileInfo.Length)} | added on {fileInfo.UploadDate}");
        }

        if (empty) Console.WriteLine("binderbox is empty");
    }

    public static void Mem()
    {
        using var db = new LiteDatabase(binderFile);
        var storage = db.GetStorage<string>();

        long totalBytes = 0;

        foreach (var fileInfo in storage.FindAll())
        {
            totalBytes += fileInfo.Length;
        }

        Console.WriteLine($"total storage: {Commands.FormatBytes(totalBytes)}");
    }

    public static void Clear()
    {
        using var db = new LiteDatabase(binderFile);
        var storage = db.GetStorage<string>();

        var allFiles = storage.FindAll().ToList();

        if (!allFiles.Any())
        {
            Console.WriteLine("Binderbox is already empty");
            return;
        }

        Console.WriteLine($"warning: this will permanently delete {allFiles.Count} file(s) from Binderbox");
        Console.Write("continue? (y/n): ");
        var input = Console.ReadLine();

        if (input?.ToLower() != "y")
        {
            Console.WriteLine("cancelled");
            return;
        }

        foreach (var file in allFiles)
        {
            storage.Delete(file.Id);
        }

        Console.WriteLine("Binderbox has been cleared");
    }
}