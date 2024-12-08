using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace FurnitureMod;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess(GameProcessName)]
public class FurnitureModPlugin : BasePlugin
{
    private const string GameProcessName = "Rune Factory 5.exe";

    internal static new ManualLogSource Log = BepInEx.Logging.Logger.CreateLogSource("FurnitureMod");

    internal void LoadConfig()
    {
        FurnitureChangesPatch.LoadConfig(Config);
    }

    public override void Load()
    {
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} {MyPluginInfo.PLUGIN_VERSION} is loading!");

        LoadConfig();
        Harmony.CreateAndPatchAll(typeof(FurnitureChangesPatch));

        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} {MyPluginInfo.PLUGIN_VERSION} is loaded!");
    }
}
