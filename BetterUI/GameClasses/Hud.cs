using BepInEx.Configuration;
using BetterUI.Patches;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BetterUI.GameClasses
{
    [HarmonyPatch]
    public static class BetterHud
    {
        private static Player _player = null;
        private static Vector3 lastMousePos = Vector3.zero;
        private static float lastScrollPos = 0f;

        private static string currentlyDragging = "";
        private static bool isEditing = false;
        private static int activeLayer = 0;

        // cache the value, so you need to relog to toggle it
        private static bool enablePlayerHudEditing;

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Hud), "Awake")]
        private static void Awake(ref Hud __instance)
        {
            enablePlayerHudEditing = Main.enablePlayerHudEditing.Value;

            // Load custom elements, before getting positions
            if (Main.showCharacterXP.Value && Main.showCharacterXpBar.Value)
            {
                XPBar.Create(__instance);
            }

            if (Main.customHealthBar.Value != Main.CustomBarState.off)
            {
                CustomBars.HealthBar.Create();
            }

            if (Main.customStaminaBar.Value != Main.CustomBarState.off)
            {
                CustomBars.StaminaBar.Create();
            }

            if (Main.customEitrBar.Value != Main.CustomBarState.off)
            {
                CustomBars.EitrBar.Create();
            }

            if (Main.customFoodBar.Value != Main.CustomBarState.off)
            {
                CustomBars.FoodBar.Create();
            }

            if (enablePlayerHudEditing)
            {
                // Try to support QuickSlots
                Compatibility.QuickSlotsHotkeyBar.Unanchor(__instance);

                CustomHud.Load(__instance);
                CustomHud.PositionTemplates();
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Hud), "Update")]
        private static void Update(Hud __instance)
        {
            Player localPlayer = Player.m_localPlayer;

            if (Main.showCharacterXP.Value && _player == null && localPlayer != null)
            {
                _player = localPlayer;
                XP.Awake(localPlayer);
                XPBar.UpdateLevelProgressPercentage();
            }

            if (!enablePlayerHudEditing || localPlayer == null)
            {
                return;
            }

            if (Input.GetKeyDown(Main.togglePlayerHudEditModeKey.Value))
            {
                isEditing = !isEditing;
                CustomHud.ShowTemplates(isEditing, activeLayer);
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"HUD editing is turned {(isEditing ? "ON" : $"OFF")}");
            }

            if (!isEditing)
            {
                return;
            }
            else if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                activeLayer = activeLayer == CustomHud.roots.Count ? 0 : activeLayer + 1;
                Helpers.DebugLine($"Layer changed to: {(Groups)activeLayer}");
                CustomHud.ShowTemplates(isEditing, activeLayer);
                Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"Now editing: {(Groups)activeLayer}");
            }

            float gameScale = GameObject.Find("LoadingGUI").GetComponent<CanvasScaler>().scaleFactor;

            Vector3 mousePos = Input.mousePosition; // Select element / move element
            float scrollPos = Input.GetAxis("Mouse ScrollWheel"); // Change scale

            if (lastMousePos == Vector3.zero) lastMousePos = mousePos;
            if (lastScrollPos == 0f) lastScrollPos = scrollPos;

            // Could we have this list as global? And only update it every render, no need to re-create?
            List<KeyValuePair<string, RectTransform>> rectList = new List<KeyValuePair<string, RectTransform>>();

            // Check every element, if found add to rectList that is then later scanned if mouse is on them.
            // Could we use groups, and then toggle between groups. Ex. activeGroup param.
            foreach (HudElement e in CustomHud.elements)
            {
                try
                {
                    // Get elements only from active layer.
                    RectTransform rt = CustomHud.LocateTemplateRect(e.Name);
                    // 'rt' will get every template from different layers
                    // Add only rects from active layer
                    if (rt && e.Group == (Groups)activeLayer)
                    {
                        rectList.Add(new KeyValuePair<string, RectTransform>(e.Name, rt));
                    }
                }
                catch
                {
                    Helpers.DebugLine($"Issues while locating UI templates. Your uiData might be corrupted.\nIssue on: {e.Name} ({e.DisplayName})");
                }
            }

            Vector3 mousePositionChange = (mousePos - lastMousePos) / gameScale;

            if (Helpers.CheckHeldKey(Main.modKeyPrimary.Value) && rectList.Count > 0)
            {
                if (currentlyDragging != string.Empty)
                {
                    if (Helpers.CheckHeldKey(Main.modKeySecondary.Value))
                    {
                        Vector2 scaledMousePositionChange = mousePositionChange / (new Vector2(Screen.currentResolution.width, Screen.currentResolution.height) / 10);

                        CustomHud.UpdateScaleAndDimensions(currentlyDragging, scaledMousePositionChange, scrollPos);
                    }
                    else
                    {
                        CustomHud.UpdatePosition(currentlyDragging, mousePositionChange);
                        ConfigEntry<Main.CustomBarState> entry = null;

                        switch (currentlyDragging)
                        {
                            case CustomBars.FoodBar.objectName:
                                entry = Main.customFoodBar;
                                break;

                            case CustomBars.HealthBar.objectName:
                                entry = Main.customHealthBar;
                                break;

                            case CustomBars.StaminaBar.objectName:
                                entry = Main.customStaminaBar;
                                break;

                            case CustomBars.EitrBar.objectName:
                                entry = Main.customEitrBar;
                                break;

                            default:
                                break;
                        }

                        // TODO you could also add rotation support to the buff bar and the hotbar here
                        if (entry != null)
                        {
                            if (scrollPos > 0)
                            {
                                entry.Value = CustomBars.BarHelper.IncrementRotation(entry.Value);
                            }
                            else if (scrollPos < 0)
                            {
                                entry.Value = CustomBars.BarHelper.DecrementRotation(entry.Value);
                            }
                        }
                    }
                }
                else
                {
                    foreach (KeyValuePair<string, RectTransform> item in rectList)
                    {
                        if (RectTransformUtility.RectangleContainsScreenPoint(item.Value, mousePos))
                        {
                            currentlyDragging = item.Key;
                            break;
                        }
                    }
                }
            }
            else
            {
                currentlyDragging = string.Empty;
            }

            lastMousePos = mousePos;
            lastScrollPos = scrollPos;
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Hud), "UpdateHealth")]
        private static void UpdateHealth(Player player)
        {
            // the class will decide if it should do something, ignore config values here
            CustomBars.HealthBar.Update(player.GetMaxHealth(), player.GetHealth());
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Hud), "UpdateStamina")]
        private static void UpdateStamina(Player player)
        {
            // the class will decide if it should do something, ignore config values here
            CustomBars.StaminaBar.Update(player.GetMaxStamina(), player.GetStamina());
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Hud), "UpdateEitr")]
        private static void UpdateEitr(Player player)
        {
            // the class will decide if it should do something, ignore config values here
            CustomBars.EitrBar.Update(player.GetMaxEitr(), player.GetEitr());
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Hud), "UpdateFood")]
        private static void UpdateFood(Player player)
        {
            // the class will decide if it should do something, ignore config values here
            CustomBars.FoodBar.Update(player);
        }
    }
}