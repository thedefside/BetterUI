﻿using BetterUI.Patches;
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

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Hud), "Awake")]
        private static void Awake(ref Hud __instance)
        {
            if (Main.showCharacterXP.Value && Main.showCharacterXpBar.Value)
            {
                XPBar.Create(__instance);
            }

            if (Main.enablePlayerHudEditing.Value)
            {
                // Try to support QuickSlots
                // TODO this could also be done after creating the custom elements right?
                Compatibility.QuickSlotsHotkeyBar.Unanchor(__instance);
            }

            // Load custom elements, before getting positions
            if (Main.useCustomHealthBar.Value)
            {
                CustomElements.HealthBar.Create();
            }

            if (Main.useCustomStaminaBar.Value)
            {
                CustomElements.StaminaBar.Create();
            }

            if (Main.useCustomSpoilerBar.Value)
            {
                CustomElements.EitrBar.Create();
            }

            if (Main.useCustomFoodBar.Value)
            {
                CustomElements.FoodBar.Create();
            }

            if (Main.enablePlayerHudEditing.Value)
            {
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

            if (!Main.enablePlayerHudEditing.Value || localPlayer == null)
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
                    RectTransform rt = CustomHud.LocateTemplateRect(e.group, e.name);
                    // 'rt' will get every template from different layers
                    // Add only rects from active layer
                    if (rt && e.group == (Groups)activeLayer)
                    {
                        rectList.Add(new KeyValuePair<string, RectTransform>(e.name, rt));
                    }
                }
                catch
                {
                    Helpers.DebugLine($"Issues while locating UI templates. Your uiData might be corrupted.\nIssue on: {e.name} ({e.displayName})");
                }
            }

            if (Helpers.CheckHeldKey(Main.modKeyPrimary.Value) && rectList.Count > 0)
            {
                if (currentlyDragging != "")
                {
                    var item = rectList.Find(e => e.Key == currentlyDragging);
                    if (item.Key == currentlyDragging)
                    {
                        if (Helpers.CheckHeldKey(Main.modKeySecondary.Value))
                            CustomHud.UpdateDimensions(item.Key, scrollPos);
                        else
                            CustomHud.UpdatePosition(item.Key, (mousePos - lastMousePos) / gameScale, scrollPos);
                    }
                }
                else
                {
                    foreach (var item in rectList)
                    {
                        if (RectTransformUtility.RectangleContainsScreenPoint(item.Value, mousePos))
                        {
                            CustomHud.UpdatePosition(item.Key, (mousePos - lastMousePos) / gameScale, scrollPos);
                            currentlyDragging = item.Key;
                            break;
                        }
                    }
                }
            }
            else currentlyDragging = "";

            lastMousePos = mousePos;
            lastScrollPos = scrollPos;

            // For XP Bar Debug
            /*
            if (_bar != null)
            {
              if (_bar.m_value >= 1f) _bar.m_value = 0f;
              _bar.SetValue(_bar.m_value + 0.001f);
            }
            */
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Hud), "UpdateHealth")]
        private static void UpdateHealth(Player player)
        {
            // the class will decide if it should do something, ignore config values here
            CustomElements.HealthBar.Update(player.GetMaxHealth(), player.GetHealth());
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Hud), "UpdateStamina")]
        private static void UpdateStamina(Player player)
        {
            // the class will decide if it should do something, ignore config values here
            CustomElements.StaminaBar.Update(player.GetMaxStamina(), player.GetStamina());
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Hud), "UpdateEitr")]
        private static void UpdateEitr(Player player)
        {
            // the class will decide if it should do something, ignore config values here
            CustomElements.EitrBar.Update(player.GetMaxEitr(), player.GetEitr());
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Hud), "UpdateFood")]
        private static void UpdateFood(Player player)
        {
            // the class will decide if it should do something, ignore config values here
            CustomElements.FoodBar.Update(player);
        }
    }
}