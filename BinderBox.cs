using LiteDB;

public static class BinderBox
{
    private static readonly string binderFile = Path.Combine(AppContext.BaseDirectory, "binderbox.bbox");

    public static void Add(string path)
    {
        path = Path.GetFullPath(path);

        if (!File.Exists(path))
        {
            Console.WriteLine("file not found in binderbox");
            return;
        }

        string fileName = Path.GetFileName(path);

        using var db = new LiteDatabase(binderFile);
        var storage = db.GetStorage<string>();

        storage.Delete(fileName);

        storage.Upload(fileName, path);

        File.Delete(path);

        Console.WriteLine($"added file to binderbox: {fileName}");
    }

    public static void Extract(string fileName)
    {
        using var db = new LiteDatabase(binderFile);
        var storage = db.GetStorage<string>();

        if (!storage.Exists(fileName))
        {
            Console.WriteLine("file not found in binderbox");
            return;
        }

        string outPath = Path.Combine(Globals.currentDir, fileName);

        using var outFile = File.Create(outPath);
        storage.Download(fileName, outFile);

        storage.Delete(fileName);

        Console.WriteLine($"extracted file from binderbox: {fileName}");
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
}