# Transformers plugin/mod for Star Trek Fleet Command™ game client
Current version: [v1.5.0](https://github.com/Plurimus/Optimus.STFC.Transformers/releases/tag/v1.5.0)
Tested with STFC client v1.000.51558 (Update 94.0.0)
Designed to use with [BepInEx.STFC](https://builds.bepinex.dev/projects/bepinex_be) framework (bleeding edge builds)
Recommended to use with [BepInExConfigManager.Il2Cpp](https://github.com/Jelosus2/BepInExConfigManager) plugin as ingame settings editor

## What it does
This mod changes some interface windows/pop-ups/menus:
- Extended speedups inventory popup menu
- Extended combat report window
- Extended ship management stats window
- Change pop-up object viewer scale (ships, systems, armadas)
- Always show names and tags of ships on any zoom
- Root Scale change (overall UI scale) — **off by default**
- Zoom extended (increased camera zoom limits) — **off by default**

`Root Scale change` and `Zoom extended` are disabled by default because they patch game methods (`ScreenManager.UpdateCanvasRootScaleFactor` and `NavigationZoom.SetZoomLevel`) that may conflict with Community Patch Mod. Enable them in the config if you don't use Community Patch Mod (or don't rely on it for the same features) — the game needs to be restarted after changing either setting, since the mod decides whether to apply the patch only once, on startup.

## How to install
- unpack BepInEx framework release archive into STFC game folder which is usually located at `C:\Games\Star Trek Fleet Command\Star Trek Fleet Command\default\game`
- run the game client and wait until BepInEx will make interop assemblies in `..\BepInEx\interop\` folder
- unpack Optimus.STFC.Transformers archive to BepInEx folder so it will copy `Optimus.STFC.Transformers.dll` to `C:\Games\Star Trek Fleet Command\Star Trek Fleet Command\default\gameBepInEx\plugins\`
- make a link file to `prime.exe` and start the game client with it to avoid automatic updates 

## Used libraries
- [BepInEx/BepInEx.Unity.IL2CPP](https://nuget.bepinex.dev/packages/BepInEx.Unity.IL2CPP) - v6.0.0-be.755
- [BepInEx/BepInEx.PluginInfoProps](https://nuget.bepinex.dev/packages/BepInEx.PluginInfoProps) - v2.1.0

## Support
For STFC BepInEx items, please visit the [BORG Box](https://discord.gg/8MRcfserGH) discord server.

## Result screenshots
![Transformers1](Transformers1.png)
![Transformers2](Transformers2.png)
![Transformers3](Transformers3.png)
![Transformers4](Transformers4.png)
![Transformers5](Transformers5.png)



