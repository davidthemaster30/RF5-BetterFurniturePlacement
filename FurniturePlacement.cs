using BepInEx;
using BepInEx.Configuration;
using BepInEx.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace FurnitureMod;

[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
[BepInProcess(GameProcessName)]
public partial class FurnitureMod : BasePlugin
{
    #region PluginInfo
    internal const string pluginGuid = "BetterFurniturePlacementMod";
    internal const string pluginName = "Better Furniture Placement Mod";
    internal const string pluginVersion = "0.5";
    private const string GameProcessName = "Rune Factory 5.exe";
    #endregion

    internal static new ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource("FurnitureMod");

    internal static ConfigEntry<bool> bFurnitureSnapping;
    internal static ConfigEntry<bool> bAlwaysIgnore;
    internal static ConfigEntry<KeyCode> kToggleKey;
    internal static ConfigEntry<RF5Input.Key> kToggleButton;
    internal static ConfigEntry<KeyCode> kIncreaseGrid;
    internal static ConfigEntry<KeyCode> kDecreaseGrid;

    public override void Load()
    {
        Log = base.Log;
        Log.LogInfo($"Plugin {pluginGuid} is loaded!");

        bAlwaysIgnore = Config.Bind("Always Ignore Placement Collison", "AlwaysIgnore", false, "Set to true to always Ignore Furniture Placement Collison");
        bFurnitureSnapping = Config.Bind("Furniture Snap to Grid", "SnapToGrid", true, "Set to true to snap furniture to a grid");
        kToggleKey = Config.Bind("Key to Ignore Placement", "IgnoreKey", KeyCode.LeftShift, "Set the Key you wish to press while Holding Furniture to Ignore Placement Collison (Not required if AlwaysIgnore is true)");
        kToggleButton = Config.Bind("Button to Ignore Placement", "IgnoreButton", RF5Input.Key.R, "Set the Button you wish to press while Holding Furniture to Ignore Placement Collison (Not required if AlwaysIgnore is true)");
        kIncreaseGrid = Config.Bind("Button to Increase Grid Size", "IncreaseGrid", KeyCode.Period, "Set the Key you wish to press to Increase grid size");
        kDecreaseGrid = Config.Bind("Button to Decrease Grid Size", "DecreaseGrid", KeyCode.Comma, "Set the Key you wish to press to Decrease grid size");

        Harmony.CreateAndPatchAll(typeof(FurnitureChanges));
    }
}
