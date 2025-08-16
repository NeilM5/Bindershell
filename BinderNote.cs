using System.Data;

public static class BinderNote
{
    private static List<string> buffer = new();
    private static int cursorX = 0, cursorY = 0;
    private static bool isEditMode = false;
    private static string commandInput = "";

    public static void RunNote()
    {
        // Initialize (6 default lines)
        buffer = new List<string> { "" };
        cursorX = 0;
        cursorY = 0;
        isEditMode = false;

        Console.Clear();
        Redraw();

        while (true)
        {
            var key = Console.ReadKey(true);

            if (isEditMode)
            {
                if (key.Key == ConsoleKey.Escape)
                {
                    isEditMode = false;
                    commandInput = "";
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (cursorX > 0)
                    {
                        buffer[cursorY] = buffer[cursorY].Remove(cursorX - 1, 1);
                        cursorX--;
                    }
                    else if (cursorY > 0)
                    {
                        int prevLine = buffer[cursorY - 1].Length;
                        buffer[cursorY - 1] += buffer[cursorY];
                        buffer.RemoveAt(cursorY);
                        cursorY--;
                        cursorX = prevLine;
                    }
                }
                else if (key.Key == ConsoleKey.Enter)
                {
                    buffer.Insert(cursorY + 1, "");
                    cursorY++;
                    cursorX = 0;
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    buffer[cursorY] = buffer[cursorY].Insert(cursorX, key.KeyChar.ToString());
                    cursorX++;
                }
            }
            // command mode
            else
            {
                if (key.Key == ConsoleKey.Enter)
                {
                    string[] cmd = commandInput.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);

                    if (cmd.Length > 0)
                    {
                        switch (cmd[0])
                        {
                            case "help":
                                Help();
                                break;
                            case "save":
                                Save("bindernote.txt");
                                break;
                            case "save-as":
                                if (cmd.Length > 1) Save(cmd[1]);
                                else Save("bindernote.txt");
                                break;
                            case "quit":
                                Console.Clear();
                                Console.Write("quit (unsaved progress will be lost)? (y/n): ");
                                var confirm = Console.ReadKey(true);
                                if (confirm.Key == ConsoleKey.Y)
                                {
                                    Console.Clear();
                                    return;
                                }
                                break;
                            case "edit":
                                isEditMode = true;
                                break;
                            default:
                                Console.SetCursorPosition(0, Console.WindowHeight - 3);
                                Console.WriteLine($"unknown Bindernote command: {cmd[0]}");
                                break;
                        }
                    }

                    commandInput = "";
                }
                else if (key.Key == ConsoleKey.Backspace)
                {
                    if (commandInput.Length > 0)
                    {
                        commandInput = commandInput[..^1];
                    }
                }
                else
                {
                    commandInput += key.KeyChar;
                }
            }

            ClampCursor();
            Redraw();
        }
    }

    private static void Help()
    {
        Console.Clear();
        Console.WriteLine(@"
        Bindernote
        ----------

        usage: [command] [args]

        help                        show list of available Bindernote commands
        save                        save without quiting (default bindernote.txt)
        save-as [filename]          save as the filename provided (.txt)
        quit                        quit (with warning)
        edit                        return to edit mode
        ");
        Console.Write("\npress any key to continue...");
        Console.ReadKey(true);
    }

    private static void Save(string filename)
    {
        File.WriteAllLines(filename, buffer);
    }

    private static void Redraw()
    {
        Console.Clear();

        // draw buffer
        for (int i = 0; i < buffer.Count; i++)
        {
            if (string.IsNullOrEmpty(buffer[i]))
            {
                Console.WriteLine("-");
            }
            else Console.WriteLine(buffer[i]);
        }

        // seperator
        Console.SetCursorPosition(0, Console.WindowHeight - 2);
        Console.WriteLine(new string('-', Console.WindowWidth));

        // ui bar
        Console.SetCursorPosition(0, Console.WindowHeight - 1);
        if (isEditMode)
        {
            Console.Write($"{Themes.GetColor("cmdColor")}EDIT{Themes.GetColor("end")}  |  ln {cursorY}, col {cursorX}");
        }
        else
        {
            Console.Write($"{Themes.GetColor("cmdColor")}COMMAND{Themes.GetColor("end")} > {commandInput}");
        }

        // set cursor position
        if (isEditMode)
        {
            Console.SetCursorPosition(cursorX, cursorY);
        }
    }

    private static void ClampCursor()
    {
        if (cursorY < 0) cursorY = 0;
        if (cursorY >= buffer.Count) cursorY = buffer.Count - 1;

        if (cursorX < 0) cursorX = 0;
        if (cursorX > buffer[cursorY].Length) cursorX = buffer[cursorY].Length;
    }
}