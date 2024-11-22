using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace FurnitureMod;

[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
[BepInProcess(GameProcessName)]
public class FurnitureModPlugin : BasePlugin
{
    #region PluginInfo
    internal const string pluginGuid = "BetterFurniturePlacementMod";
    internal const string pluginName = "Better Furniture Placement Mod";
    internal const string pluginVersion = "0.5";
    private const string GameProcessName = "Rune Factory 5.exe";
    #endregion

    internal static new ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource("FurnitureMod");

    public override void Load()
    {
        Log.LogInfo($"Plugin {pluginGuid} is loaded!");

        FurnitureChangesPatch.LoadConfig(Config);

        Harmony.CreateAndPatchAll(typeof(FurnitureChangesPatch));
    }
}
