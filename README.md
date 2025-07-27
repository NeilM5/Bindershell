# Bindershell
A file management Command Line Interface (CLI) which allows you to navigate, operate, check, organize, and even store files and folder.
Bindershell has a built in Virtual File System (VFS) which uses [LiteDB](https://www.litedb.org/) to store files/folders.

---

## Features
- navigate and investigate files and folders
- Operate on files/folders: create, delete, move, copy, etc.
- Check file/folder information such as memory, all files of type, etc.
- Organize files/folders by type
- Store files/folders in VFS

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

### Features
- Add files to BinderBox
- Extract files from BinderBox
- List files with name, size, and data added
- Check size of BinderBox
- And more...!
