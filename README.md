# Rune Factory 5 - Better Furniture Placement

![Better Placement](https://raw.githubusercontent.com/davidthemaster30/RF5-BetterFurniturePlacement/ab4d53afae9443b4042a191dcd8d93ab7fd5b684/screenshot.png)

Allows you to HOLD Left Shift(Default) to ignore the placement collision. This will allow you to place furniture wherever you want!
Also includes snapping to grid on by default.

## Installation
1. Install [BepInEx BE v647 or later](https://builds.bepinex.dev/projects/bepinex_be) (IL2CPP_x64)
or install other base mod BepInEx already included (like [RF5Fix v0.1.5 mod](https://github.com/Lyall/RF5Fix))
2. Get [Latest Release](https://github.com/davidthemaster30/RF5-BetterFurniturePlacement/releases)
3. Extract the contents of downloaded zip into `<GameDirectory>\BepInEx\plugins`. 
4. (Optional) Get the [BepInEx Configuration Manager](https://github.com/BepInEx/BepInEx.ConfigurationManager)

### OPTIONAL
Configure settings in "\BepInEx\config\BetterFurniturePlacementMod.cfg" or via BepInEx Configuration Manager ingame with F1.
- AlwaysIgnore - Always ignores placement collision, you don't need to hold any button.
- SnapToGrid - Toggles the snapping to grid
- IgnoreButton - Controller Button needed to be held to ignore placement collision.
- IgnoreKey - Keyboard button needed to be held to ignore placement collision.

## Known Issues
- Launching the game with the Launcher stops BepInEx from working
- Controller & Keyboard Prompts + Buttons don't play nice, need to fix.
