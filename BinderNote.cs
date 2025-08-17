using System.Data;

public static class BinderNote
{
    private static List<string> buffer = new();
    private static int cursorX = 0, cursorY = 0;
    private static bool isEditMode = false;
    private static string commandInput = "";
    private static string commandMessage = "";

    private static string currenNote = "bindernote.txt";

    public static void RunNote()
    {
        // Initialize (6 default lines)
        cursorX = 0;
        cursorY = 0;
        isEditMode = false;

        if (File.Exists(currenNote))
        {
            buffer = File.ReadAllLines(currenNote).ToList();
            if (buffer.Count == 0) buffer.Add("");
        }
        else
        {
            buffer = new List<string> { "" };
        }

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
                    commandMessage = "";
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
                            case "h":
                            case "help":
                                Help();
                                break;

                            case "s":
                            case "save":
                                if (cmd.Length > 2 && cmd[1] == "-a")
                                {
                                    Save(cmd[2]);
                                    commandMessage = $"saved as: {cmd[2]}";
                                }
                                else
                                {
                                    Save();
                                    commandMessage = $"saved: {currenNote}";
                                }
                                break;
                            
                            case "o":
                            case "open":
                                if (cmd.Length > 1)
                                {
                                    string filename = cmd[1];
                                    if (File.Exists(filename))
                                    {
                                        buffer = File.ReadAllLines(filename).ToList();
                                        if (buffer.Count == 0) buffer.Add("");

                                        currenNote = filename;

                                        commandMessage = $"opened: {currenNote}";
                                    }
                                    else commandMessage = "file does not exist";
                                }
                                else commandMessage = "usage: o [filename]";
                                break;

                            case "f":
                            case "find":
                                if (cmd.Length >= 4 && (cmd[2] == "r" || cmd[2] == "replace"))
                                {
                                    string findWord = cmd[1];
                                    string replaceWord = cmd[3];
                                    int count = 0;

                                    for (int i = 0; i < buffer.Count; i++)
                                    {
                                        if (buffer[i].Contains(findWord))
                                        {
                                            buffer[i] = buffer[i].Replace(findWord, replaceWord);
                                            count++;
                                        }
                                    }

                                    commandMessage = $"replaced {count} occurrence(s) of '{findWord}' with '{replaceWord}'";
                                }
                                else commandMessage = "usage: f [old] r [new]";
                                break;

                            case "q":
                            case "quit":
                                Console.Clear();
                                Console.Write("quit (unsaved progress will be lost)? (y/n): ");
                                var confirm = Console.ReadKey(true);
                                if (confirm.Key == ConsoleKey.Y)
                                {
                                    commandInput = "";
                                    commandMessage = "";
                                    Console.Clear();
                                    return;
                                }
                                break;

                            case "e":
                            case "edit":
                                isEditMode = true;
                                break;

                            default:
                                commandMessage = $"unknown command: {cmd[0]}";
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

        usage: [command] [options] [args]
        commands have short and long forms

        h / help                                            show list of available Bindernote commands
        s / save                                            save without quiting (default bindernote.txt)
        s / save [-a] [filename]                            save as the filename provided
        o / open [filename]                                 opens a file
        f / find [old] r / replace [new]                    find and replace
        q / quit                                            quit (with warning)
        e / edit                                            return to edit mode

        press esc to return to command mode from edit mode
        ");
        Console.Write("\npress any key to continue...");
        Console.ReadKey(true);
    }

    private static void Save(string? filename = null)
    {
        if (!string.IsNullOrEmpty(filename))
        {
            currenNote = filename;
        }

        File.WriteAllLines(currenNote, buffer);
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
            Console.Write($"{Themes.GetColor("cmdColor")}EDIT{Themes.GetColor("end")} [{Themes.GetColor("dirColor")}{currenNote}{Themes.GetColor("end")}]  |  ln {cursorY}, col {cursorX}");
        }
        else
        {
            string message = string.IsNullOrEmpty(commandMessage) ? "" : $"{commandMessage}";
            Console.Write($"{Themes.GetColor("cmdColor")}COMMAND{Themes.GetColor("end")} > {commandInput}  |  {message}");

            int cursorPos = $"COMMAND > ".Length + commandInput.Length;
            Console.SetCursorPosition(cursorPos, Console.WindowHeight - 1);
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