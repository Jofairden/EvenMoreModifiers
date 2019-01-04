This folder should contain certain DLL files by default, which are referenced in the main project
However, the project also use the following files which may be missing:

- Terraria.exe.lnk or Terraria.exe.symlink
- TerrariaServer.exe.ln or TerrariaServer.exe.symlink

There two files are two file symbolink links that point to the tModLoaderDebug.exe and tModLoaderServerDebug.exe files respectively
The purpose of these files is to allow every modder to be able to debug and build the project, without having to mess with paths in project files

In order to create the symbolic links, you must run the commands below in a command prompt (Windows)
Of course, edit the paths to the correct paths

mklink "Path\To\Steam\steamapps\common\Terraria\tModLoaderDebug.exe" "Path\To\Mod Sources\Loot\references\Terraria.exe"
mklink "Path\To\Steam\steamapps\common\Terraria\tModLoaderServerDebug.exe" "Path\To\Mod Sources\Loot\references\TerrariaServer.exe"

These commands will create the file symbolic links. You can of course also point at the non debug exe variants, but some versions of the project
source may not work with them. If you dont have the debug variants you need to setup the tML source, open the solution and build 'WindowsDebug' along
with 'WindowsServerDebug'. If you want to compile in-game you will also need to build the Mac variant.