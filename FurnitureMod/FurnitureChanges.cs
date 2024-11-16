using HarmonyLib;
using UnityEngine;

namespace FurnitureMod;

public partial class FurnitureMod
{
    internal static bool shadowIgnore = false;
    internal static GameObject newButton;
    internal static float gridSize { get; set; } = 0.25f;
    internal const float gridIncrement = 0.05f;
    internal const float gridMax = 1f;
    internal const int NinetyDegrees = 90;

    [HarmonyPatch]
    public static class FurnitureChanges
    {
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
                int testKeyImage = Convert.ToInt32(Enum.Parse(typeof(ButtonSpriteManager.KeyImageType), kToggleButton.Value.ToString()));

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

                if (bAlwaysIgnore.Value)
                {
                    shadowIgnore = true;
                }
                else
                {
                    if (UnityEngine.Input.GetKeyDown(kToggleKey.Value) || RF5Input.Pad.Edge(kToggleButton.Value))
                    {
                        shadowIgnore = true;
                    }

                    if (UnityEngine.Input.GetKeyUp(kToggleKey.Value) || RF5Input.Pad.End(kToggleButton.Value))
                    {
                        shadowIgnore = false;
                    }
                }

                if (UnityEngine.Input.GetKeyDown(kIncreaseGrid.Value))
                {
                    gridSize = Mathf.Clamp(gridSize + gridIncrement, gridIncrement, gridMax);
                }
                if (UnityEngine.Input.GetKeyUp(kDecreaseGrid.Value))
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
                if (bFurnitureSnapping.Value)
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
            if (bFurnitureSnapping.Value)
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
}
