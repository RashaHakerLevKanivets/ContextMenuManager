# **Context Menu Manager - Utility for expanding the Explorer context menu**

## **Project Description**
The utility adds useful actions to the Windows Explorer context menu, allowing you to quickly perform frequently used operations without having to manually enter commands.

## **Features**
- Open command prompt (CMD) in the current directory
- Open PowerShell in the current directory
- Open WSL (Windows Subsystem for Linux) in the current directory
- Create new files with the name and extension
- Manage available actions via the graphical interface

## **System requirements**
- Operating system: Windows 7/10/11
- .NET Framework 4.0 or higher
- Administrator rights to modify the system registry

## **Installation**
1. Download the latest version of the program from the [Releases] section (https://github.com/RashaHakerLevKanivets/ContextMenuManager/releases)
2. Run the ContextMenuManager.exe file with administrator rights
3. In the interface that opens, select the desired actions
4. Click the "Save" button to apply the changes

## **Compilation from source code**
For self-assembly, you will need C# compiler (csc.exe):

```bash
csc /target:winexe /reference:System.Windows.Forms.dll /out:ContextMenuManager.exe ContextMenuManager.cs
```

## **Adding new commands**
To extend functionality:
1. Edit the ContextMenuManager.cs file
2. Add a new command to the SaveSettings method:
```csharp
ToggleContextMenu("CommandName", true, "Menu Text", "Command to execute");
```
3. Implement command handling in the HandleCommand method

## **Contributions**
Welcome:
- Bug reports via Issues
- Pull requests with improvements
- Suggestions for new functionality
