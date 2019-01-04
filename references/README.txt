This folder should contain certain DLL files by default, which are referenced in the main project
However, the project also use the following file(s) which may be missing:

- Terraria folder junction

The purpose of these files is to allow every modder to be able to debug and build the project, without having to mess with paths in project files

In order to create the symbolic links, you must run the commands below in a command prompt (Windows)
Of course, edit the paths to the correct paths

mklink /J "Path\To\Mod Sources\Loot\references\Terraria" "Path\To\Steam\steamapps\common\Terraria\"

This command will create the folder directory junction

In order to be able to build the mod, you will also need the weak dependencies as local mods (so their .tmod files in the mods folder). Download the weak referenced mods from the browser before building. If you want to see build output in VS navigate to 'View -> Output' (ALT+2)
