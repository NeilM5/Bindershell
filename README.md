# Bindershell

![Latest Release](https://img.shields.io/github/v/release/NeilM5/BinderShell?label=latest%20version)
![.NET](https://img.shields.io/badge/.NET-9.0-blueviolet)
![Platform](https://img.shields.io/badge/platform-Windows-green)
![License](https://img.shields.io/github/license/NeilM5/BinderShell)
![Last Commit](https://img.shields.io/github/last-commit/NeilM5/BinderShell)
![Issues](https://img.shields.io/github/issues/NeilM5/BinderShell)
![Stars](https://img.shields.io/github/stars/NeilM5/BinderShell?style=social)
![Forks](https://img.shields.io/github/forks/NeilM5/BinderShell?style=social)

A file management Command Line Interface (CLI) which allows you to navigate, operate, check, organize, and even store files.
Bindershell has a built in Virtual File System (VFS) called **BinderBox**, which uses [LiteDB](https://www.litedb.org/) to store files securely.

---

## Features
- navigate and investigate files and folders
- Operate on files/folders: create, delete, move, copy, etc.
- Check file/folder information (size, type, etc.)
- Organize files/folders by extension or type
- Store files/folders in Virtual File System (BinderBox)

---

## How to Run
### Run from Source
> Prerequisites: [.NET](https://dotnet.microsoft.com/download) (version 8+)
1. Clone the Repository
```bash
git clone https://github.com/NeilM5/Bindershell.git
cd Bindershell
```
2. Build and Run
```bash
dotnet run
```
### Run Executable (Windows)
> Recommended for users; no need to install .NET SDK
1. Download the latest release from [Releases](https://github.com/NeilM5/Bindershell/releases)
2. Extract the `.zip` file
3. Double-click `Bindershell.exe` or run from terminal:
```bash
./Bindershell.exe
```

### Notes
- Current directory starts at `C:\` by default
- Run `help` inside Bindershell for a full commands list

---

## BinderBox (VFS)
Binderbox is the built-in Virtual File System (VFS) accessed through Bindershell.

All files/folders are saved on binderbox.bbox which is created when first running `bbox` commands.

### BBox Features
- Add files to BinderBox
- Extract files from BinderBox
- List files with name, size, and data added
- Check total size of BinderBox
- Designed for secure and convenient embedded file storage

---

## Example Commands
- `cd [path]` - change directory
- `tree` - shows the directory struture (gives warning for large directories)
- `create -f [filename]` - creates a file in current directory (use -d for making folders)
- `del -f [filename]` - permanently delete a file in current directory with warning (use -d for deleting folders)
- `mem [filename]` - returns the size of a file
- `find [extension]` - finds and lists the paths of all files from the current directory with given extension
- `sort -f -t` - lists all files (-f) sorted by type (-t) (use `help` command to check `sort` options)
- `bbox add [filename]` - add a file to BinderBox
- `bbox list` - list all files in BinderBox with metadata

Use `help` command to view all supported commands and their options

---

## License
This project is licensed under the GNU GPL v3.0 License.

See the [LICENSE](https://github.com/NeilM5/Bindershell?tab=GPL-3.0-1-ov-file) for full terms.


