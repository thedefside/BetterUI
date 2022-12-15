using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BetterUI.Patches
{
    internal static class CustomHud
    {
        public static List<HudElement> elements;
        public static Dictionary<Groups, Transform> roots = new Dictionary<Groups, Transform>();
        public static Dictionary<Groups, Transform> templates = new Dictionary<Groups, Transform>();
        private static Transform hudRoot;
        private static Transform invRoot;
        private static Transform baseRoot;
        public static readonly string templateSuffix = "_template";

        // In the future give users ability to add new elements?
        private static readonly Element[] supportedElements =
        {
      new Element("HotKeyBar", Groups.HudRoot),
      new Element("BuildHud", Groups.HudRoot, "BuildHud/SelectedInfo"),
      new Element("MiniMap", Groups.HudRoot, "MiniMap/small"),
      new Element("GuardianPower", Groups.HudRoot),
      new Element("StatusEffects", Groups.HudRoot),
      new Element("SaveIcon", Groups.HudRoot),
      new Element("BadConnectionIcon", Groups.HudRoot),
      new Element("BuildHints", Groups.HudRoot, "KeyHints/BuildHints"),
      new Element("CombatHints", Groups.HudRoot, "KeyHints/CombatHints"),
      new Element("Player", Groups.Inventory, "Player", "PlayerInventory"),
      new Element("Container", Groups.Inventory, "Container", "ChestContainer"),
      new Element("Info", Groups.Inventory, "Info", "UITab"),
      new Element("Crafting", Groups.Inventory, "Crafting", "CraftingWindow"),
      new Element(CustomBars.HealthBar.objectName, Groups.HudRoot, CustomBars.HealthBar.objectName, "HP Bar"),
      new Element(CustomBars.FoodBar.objectName, Groups.HudRoot, CustomBars.FoodBar.objectName, "Food Bar"),
      new Element(CustomBars.StaminaBar.objectName, Groups.HudRoot, CustomBars.StaminaBar.objectName, "Stamina Bar"),
      new Element(CustomBars.EitrBar.objectName, Groups.HudRoot, CustomBars.EitrBar.objectName, "Eitr Bar"),
      new Element("QuickSlots", Groups.HudRoot, "QuickSlotsHotkeyBar", "QuickSlots")
      //new Element("QuickSlotsHotkeyBar", Groups.HudRoot, "healthpanel/Health/QuickSlotsHotkeyBar", "QuickSlotsHotkey"),
      //new Element("QuickSlotGrid", Groups.Inventory, "Player/QuickSlotGrid", "QuickSlots"),
      //new Element("EquipmentSlotGrid", Groups.Inventory, "Player/EquipmentSlotGrid", "EquipmentSlots"),
    };

        // If new items are added to mandatory items, check if user has them - if not add them.
        public static void Load(Hud hud)
        {
            try
            {
                hudRoot = hud.transform.Find("hudroot");
                invRoot = InventoryGui.instance.transform.Find("root"); // Issue, this element is hidden when inventory is closed
                baseRoot = MessageHud.instance.transform; // This layer will be projected over other UI elements

                roots[Groups.HudRoot] = hudRoot;
                roots[Groups.Inventory] = invRoot;

                if (Main.uiData.Value == "none" || Main.uiData.Value == "")
                {
                    Helpers.DebugLine($"User has no uiData. Creating basic template.");
                    elements = new List<HudElement>();
                }
                else
                {
                    try
                    {
                        byte[] bytes = Convert.FromBase64String(Main.uiData.Value);
                        elements = (List<HudElement>)bytes.DeSerialize(); // Risky, as we trust the data is valid?
                        Helpers.DebugLine($"User has {elements.Count} ui elements set.");
                    }
                    catch
                    {
                        Helpers.DebugLine($"FAILED to DeSerialize uiData: {Main.uiData.Value}");
                    }

                    foreach (var item in elements)
                    {
                        item.OnAfterDeserialize();
                    }
                }

                if (elements.Count < supportedElements.Length)
                {
                    foreach (Element e in supportedElements)
                    {
                        // Element does not exist in users uiData, add it.
                        if (!elements.Exists(he => he.Name == e.Name))
                        {
                            Helpers.DebugLine($"Adding to elements: {e.Name}");
                            elements.Add(new HudElement(e.Name, e.DisplayName, e.Group, e.LocationPath, Vector2.zero));

                            if (elements.Count == supportedElements.Length) break;
                        }
                    }
                }
                else if (elements.Count > supportedElements.Length)
                {
                    // We have more elements than supported? Are there duplicates, how?
                    Helpers.DebugLine($"Seems that your UI might be corrupted!", true, true);
                }

                CreateTemplates();
            }
            catch (Exception e)
            {
                Helpers.DebugLine($"Issue while CustomHud Load. {e.Message}", true, true);
            }
        }

        public static void Save()
        {
            try
            {
                // Before saving, check if unset elements -> no need to save them.
                elements.RemoveAll(e => e.Position == Vector2.zero);
                byte[] bytes = elements.Serialize();
                Helpers.DebugLine($"uiData bytes: {bytes.Length}");
                string base64String = Convert.ToBase64String(bytes);
                Main.uiData.Value = base64String;
            }
            catch (Exception e)
            {
                Helpers.DebugLine($"FAILED to Save: {e.Message}");
            }
        }

        public static void ShowTemplates(bool show, int activeLayer)
        {
            // Try to find reason on using this?
            //roots.TryGetValue((Groups)activeLayer, out Transform activeTemplate);

            foreach (HudElement e in elements)
            {
                if ((Groups)activeLayer == e.Group)
                {
                    RectTransform rt = LocateTemplateRect(e.Name);
                    if (rt)
                    {
                        rt.gameObject.SetActive(show);
                    }
                }
                else
                {
                    RectTransform rt = LocateTemplateRect(e.Name);
                    if (rt)
                    {
                        rt.gameObject.SetActive(false);
                    }
                }
            }

            if (!show) Save();
        }

        public static void UpdatePosition(string name, Vector2 posChange)
        {
            HudElement element = elements.Find(e => e.Name == name);

            if (element.Name == name)
            {
                element.Position += posChange;

                if (element.Group == Groups.Inventory)
                {
                    var newPos = (Vector2)Camera.main.ScreenToViewportPoint(posChange);

                    element.AnchorMin += newPos;
                    element.AnchorMax += newPos;
                }

                // Update element & template position
                PositionTemplate(element);
            }
        }

        public static void UpdateScaleAndDimensions(string name, Vector2 dimensionChanges, float scaleChange)
        {
            HudElement element = elements.Find(e => e.Name == name);

            if (element.Name == name)
            {
                if (scaleChange != 0f)
                {
                    element.ChangeScale(scaleChange);
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"{element.DisplayName} size: {element.Scale}");
                }

                if (dimensionChanges != Vector2.zero)
                {
                    element.ChangeXDims(dimensionChanges.x);
                    element.ChangeYDims(dimensionChanges.y);
                    Player.m_localPlayer.Message(MessageHud.MessageType.Center, $"{element.DisplayName} dimensions: ({element.XDimensions},{element.YDimensions})");
                    // Update element & template position
                    PositionTemplate(element);
                }
            }
            else
            {
                Helpers.DebugLine($"Invalid call when updating element: {name}", true, true);
            }
        }

        private static void PositionTemplate(HudElement e)
        {
            try
            {
                RectTransform rt = LocateRectTransform(e.Group, e.Path);  // Original object
                RectTransform tt = LocateTemplateRect(e.Name);   // Your generated template
                                                                 //Helpers.DebugLine($"{rt} {rt.anchorMin} {e.GetPosition()}");
                if (rt)
                {
                    if (e.Group == Groups.Inventory)
                    {
                        float gameScale = GameObject.Find("LoadingGUI").GetComponent<CanvasScaler>().scaleFactor;
                        //Helpers.DebugLine($"\n{e.GetPosition()}\n{gameScale}\n{Camera.main.ViewportToScreenPoint(e.GetAnchorMin())}\n{tt.position}");
                        //Helpers.DebugLine($"\n{e.GetPosition() / gameScale}");
                        // Original object are moved by anchors
                        Vector3 cPos = Camera.main.ViewportToScreenPoint(e.AnchorMax);
                        Vector2 ePos = e.Position;

                        rt.anchorMin = e.AnchorMin;
                        rt.anchorMax = e.AnchorMax;
                        tt.anchoredPosition = e.Position / gameScale;
                    }
                    else
                    {
                        rt.anchoredPosition = e.Position;
                        tt.anchoredPosition = e.Position; //rt.anchoredPosition;
                    }

                    rt.localScale = new Vector3(e.Scale * e.XDimensions, e.Scale * e.YDimensions);
                    tt.localScale = rt.localScale;
                }
            }
            catch
            {
                Helpers.DebugLine($"PositionTemplate Catch: {e.Name}");
            }
        }

        public static RectTransform LocateRectTransform(Groups group, string path)
        {
            try
            {
                roots.TryGetValue(group, out Transform parent);
                // We change parent to Inventory root
                if (group == Groups.Inventory) parent = InventoryGui.instance.transform.Find("root");

                return parent.Find(path).GetComponent<RectTransform>();
            }
            catch
            {
                return null;
            }
        }

        public static RectTransform LocateTemplateRect(string name)
        {
            try
            {
                return baseRoot.Find($"{name}{templateSuffix}").GetComponent<RectTransform>();
            }
            catch
            {
                Helpers.DebugLine($"Unable to find template for {name}", true, true);
                return null;
            }
        }

        public static void PositionTemplates()
        {
            foreach (HudElement e in elements) PositionTemplate(e);
        }

        private static void CreateTemplates()
        {
            List<HudElement> unusedElements = new List<HudElement>();
            foreach (HudElement e in elements)
            {
                try
                {
                    RectTransform rt = LocateRectTransform(e.Group, e.Path);
                    if (e.Position == Vector2.zero)
                    {
                        if (e.Group == Groups.Inventory)
                        {
                            e.Position = rt.anchoredPosition;
                            // This elements depend on anchors, set these
                            e.AnchorMin = rt.anchorMin;
                            e.AnchorMax = rt.anchorMax;
                        }
                        else
                        {
                            e.Position = rt.anchoredPosition;
                        }
                    }
                    AddTemplateToHud(e, rt);
                }
                catch
                {
                    unusedElements.Add(e);
                }
            }
            // Remove unused elements from main ElementList
            if (unusedElements.Count > 0)
            {
                Helpers.DebugLine($"Removing {unusedElements.Count} unused elements.");
                foreach (HudElement e in unusedElements)
                {
                    Helpers.DebugLine($"Remove {e.DisplayName} as not used.");
                    elements.Remove(e);
                }
            }
        }

        private static void AddTemplateToHud(HudElement element, RectTransform rt)
        {
            // Should we add these to their own elements? Based on their group?
            // roots.TryGetValue(Groups.HudRoot, out Transform templateRoot); // Everything on hudRoot
            // roots.TryGetValue(element.group, out Transform templateRoot);

            Transform go = UnityEngine.Object.Instantiate(hudRoot.Find("BuildHud/SelectedInfo"), baseRoot);
            go.gameObject.name = $"{element.Name}{templateSuffix}";
            go.Find("selected_piece").gameObject.SetActive(false);
            go.Find("requirements").gameObject.SetActive(false);

            Text t = go.gameObject.AddComponent<Text>();
            t.text = $"{element.DisplayName}";
            t.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
            t.fontSize = 20;
            t.alignment = TextAnchor.MiddleCenter;
            go.gameObject.SetActive(false); // Have it hidden when added

            RectTransform templateRT = go.GetComponent<RectTransform>();
            templateRT.pivot = rt.pivot;
            templateRT.anchorMin = rt.anchorMin;
            templateRT.anchorMax = rt.anchorMax;
            templateRT.offsetMin = rt.offsetMin;
            templateRT.offsetMax = rt.offsetMax;
            templateRT.sizeDelta = rt.sizeDelta;
            templateRT.anchoredPosition = rt.anchoredPosition;
            templateRT.position = rt.position;
            templateRT.localEulerAngles = rt.localEulerAngles;
            t.resizeTextForBestFit = true;
        }
    }
}