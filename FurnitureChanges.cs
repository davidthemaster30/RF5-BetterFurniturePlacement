using BepInEx.Configuration;
using HarmonyLib;
using UnityEngine;

namespace FurnitureMod;

[HarmonyPatch]
public static class FurnitureChangesPatch
{
    internal static bool shadowIgnore = false;
    internal static GameObject newButton;
    internal static float gridSize { get; set; } = 0.25f;
    internal const float gridIncrement = 0.05f;
    internal const float gridMax = 1f;
    internal const int NinetyDegrees = 90;

    internal static ConfigEntry<bool> EnableFurnitureSnapping;
    internal static ConfigEntry<bool> EnableAlwaysIgnoreCollison;
    internal static ConfigEntry<KeyboardShortcut> ToggleKey;
    internal static ConfigEntry<RF5Input.Key> ToggleButton;
    internal static ConfigEntry<KeyboardShortcut> IncreaseGridKey;
    internal static ConfigEntry<KeyboardShortcut> DecreaseGridKey;

    private static readonly uint[] ButtonList = new List<uint>((uint[])Enum.GetValues(typeof(RF5Input.Key))).Where(x => x != (uint)RF5Input.Key.ANYKEY || x != (uint)RF5Input.Key.ALLKEY).ToArray();
    private static readonly ConfigDescription ToggleButtonDescription = new ConfigDescription(
    "Set the Button you wish to press while Holding Furniture to Ignore Placement Collison (Not required if AlwaysIgnore is true)",
    new AcceptableValueList<uint>(ButtonList));
    internal static void LoadConfig(ConfigFile Config)
    {
        EnableAlwaysIgnoreCollison = Config.Bind("Always Ignore Placement Collison", "AlwaysIgnore", false, "Set to true to always Ignore Furniture Placement Collison");
        EnableFurnitureSnapping = Config.Bind("Furniture Snap to Grid", "SnapToGrid", true, "Set to true to snap furniture to a grid");
        ToggleKey = Config.Bind("Key to Ignore Placement", "IgnoreKey", new KeyboardShortcut(KeyCode.LeftShift), "Set the Key you wish to press while Holding Furniture to Ignore Placement Collison (Not required if AlwaysIgnore is true)");
        ToggleButton = Config.Bind("Button to Ignore Placement", "IgnoreButton", RF5Input.Key.R, ToggleButtonDescription);
        IncreaseGridKey = Config.Bind("Button to Increase Grid Size", "IncreaseGrid", new KeyboardShortcut(KeyCode.Period), "Set the Key you wish to press to Increase grid size");
        DecreaseGridKey = Config.Bind("Button to Decrease Grid Size", "DecreaseGrid", new KeyboardShortcut(KeyCode.Comma), "Set the Key you wish to press to Decrease grid size");
    }

    private static void CreateIgnoreBtn()
    {
        if (newButton is null)
        {
            newButton = GameObject.Instantiate(GameObject.Find("B"), new Vector3(0, 0, 0), new Quaternion(0, 0, 0, 0), GameObject.Find("OnHandUI").transform);
            newButton.name = "ShadowIgnoreBtn";
            newButton.GetComponent<RectTransform>().anchoredPosition = GameObject.Find("B").GetComponent<RectTransform>().anchoredPosition + new Vector2(60, 80);
            newButton.GetComponent<RectTransform>().sizeDelta = new Vector2(400, 103);
            newButton.GetChild(0).GetComponent<RectTransform>().anchoredPosition += new Vector2(-57.7001f, 0);
            newButton.GetChild(1).GetComponent<RectTransform>().sizeDelta = new Vector2(400, 30);
            int testKeyImage = Convert.ToInt32(Enum.Parse(typeof(ButtonSpriteManager.KeyImageType), ToggleButton.Value.ToString()));

            newButton.GetChild(0).GetComponent<ButtonImageController>().nowButtonType = (ButtonSpriteManager.KeyImageType)testKeyImage;
            newButton.GetChild(0).GetComponent<ButtonImageController>().startButtonType = (ButtonSpriteManager.KeyImageType)testKeyImage;
            newButton.GetChild(0).GetComponent<ButtonImageController>().RefreshImage();
            newButton.GetComponentInChildren<SText>().text = "IGNORE BLOCK";
        }
    }

    [HarmonyPatch(typeof(ItemFurniture.PlayerItemFurniture2), nameof(ItemFurniture.PlayerItemFurniture2.Update))]
    [HarmonyPostfix]
    public static void Update(ItemFurniture.PlayerItemFurniture2 __instance)
    {
        if (__instance.CurrentState == ItemFurniture.PlayerItemFurniture2.State.Hold)
        {
            if (GameObject.Find("B") is not null)
            {
                CreateIgnoreBtn();
            }

            if (EnableAlwaysIgnoreCollison.Value)
            {
                shadowIgnore = true;
            }
            else
            {
                if (ToggleKey.Value.IsDown() || RF5Input.Pad.Edge(ToggleButton.Value))
                {
                    shadowIgnore = true;
                }

                if (ToggleKey.Value.IsUp() || RF5Input.Pad.End(ToggleButton.Value))
                {
                    shadowIgnore = false;
                }
            }

            if (IncreaseGridKey.Value.IsDown())
            {
                gridSize = Mathf.Clamp(gridSize + gridIncrement, gridIncrement, gridMax);
            }

            if (DecreaseGridKey.Value.IsDown())
            {
                gridSize = Mathf.Clamp(gridSize - gridIncrement, gridIncrement, gridMax);
            }
        }
    }

    [HarmonyPatch(typeof(ItemFurniture.FurnitureShadow), nameof(ItemFurniture.FurnitureShadow.updateDisp))]
    [HarmonyPostfix]
    public static void updateDisp(ItemFurniture.FurnitureShadow __instance)
    {
        if (__instance.isActive && shadowIgnore)
        {
            __instance._materials[0].SetColor("_Color", new Color(1, 1, 1, 0.5f));
        }
    }

    [HarmonyPatch(typeof(ItemFurniture.FurnitureShadow), nameof(ItemFurniture.FurnitureShadow.CheckHit))]
    [HarmonyPrefix]
    public static bool CheckHit(ItemFurniture.FurnitureShadow __instance)
    {
        if (__instance.isActive && shadowIgnore)
        {
            Vector3 oldLocation = __instance.transform.position;
            Vector3 oldRotation = __instance.transform.eulerAngles;

            __instance.isHit = !shadowIgnore;
            if (EnableFurnitureSnapping.Value)
            {
                __instance.transform.position = new Vector3(Mathf.Round(oldLocation.x / gridSize) * gridSize, oldLocation.y, Mathf.Round(oldLocation.z / gridSize) * gridSize);
                __instance.transform.eulerAngles = new Vector3(oldRotation.x, Mathf.Round(oldRotation.y / NinetyDegrees) * NinetyDegrees, oldRotation.z);
            }
        }

        return !shadowIgnore;
    }

    [HarmonyPatch(typeof(ItemFurniture.PlayerItemFurniture2), nameof(ItemFurniture.PlayerItemFurniture2.OnPutOn))]
    [HarmonyPrefix]
    public static void OnPutOn(ItemFurniture.PlayerItemFurniture2 __instance)
    {
        if (EnableFurnitureSnapping.Value)
        {
            Vector3 oldLocation = __instance.transform.position;
            Vector3 oldRotation = __instance.transform.eulerAngles;
            __instance.transform.position = new Vector3(Mathf.Round(oldLocation.x / gridSize) * gridSize, oldLocation.y, Mathf.Round(oldLocation.z / gridSize) * gridSize);
            __instance.transform.eulerAngles = new Vector3(oldRotation.x, Mathf.Round(oldRotation.y / NinetyDegrees) * NinetyDegrees, oldRotation.z);
        }

        if (newButton is not null)
        {
            GameObject.Destroy(newButton);
        }
    }
}
