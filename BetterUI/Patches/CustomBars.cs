using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static BetterUI.Main;

namespace BetterUI.Patches
{
    internal class CustomBars
    {
        public static class BarHelper
        {
            public static float StepSize => Hud.instance.m_healthPanel.sizeDelta.x;

            public const float scalingFactor = 0.6f;
            public const int padding = 0;

            public static void BaseCreate(string objectName, string origBarName, ref RectTransform root, ref GuiBar slowBar, ref GuiBar fastBar, ref TMP_Text barText)
            {
                // we've obviously already done this before if it's not null
                if (root != null)
                {
                    return;
                }

                // Hide original bar
                Hud.instance.transform.Find("hudroot").Find(origBarName).gameObject.SetActive(false);

                root = UnityEngine.Object.Instantiate(Hud.instance.m_healthBarRoot, Hud.instance.transform.Find("hudroot"));
                root.gameObject.name = objectName;

                fastBar = root.Find("fast").GetComponent<GuiBar>();
                slowBar = root.Find("slow").GetComponent<GuiBar>();

                var fastBarHealthText = fastBar.transform.Find("bar").Find("HealthText");
                fastBarHealthText.gameObject.SetActive(false);
                barText = UnityEngine.Object.Instantiate(fastBarHealthText.GetComponent<TMP_Text>(), root);
                barText.gameObject.AddComponent<TextScaler>();
                barText.gameObject.SetActive(true);

                // Resize to a more "slim" rectangle - authors preference
                root.Find("border").localScale = new Vector3(1f, scalingFactor, 1f);
                root.Find("bkg").localScale = new Vector3(1f, scalingFactor, 1f);

                fastBar.transform.localScale = new Vector3(1f, scalingFactor, 1f);
                slowBar.transform.localScale = new Vector3(1f, scalingFactor, 1f);
            }

            public static void UpdateRotation(int configRotation, ref RectTransform root, ref TMP_Text barText)
            {
                //configRotation = (configRotation + 180) % 360;

                root.localEulerAngles = new Vector3(0, 0, configRotation);
                barText.transform.localEulerAngles = new Vector3(0, 0, -configRotation);
            }

            public static void HealthStyleUpdate(float max, float current, ref GuiBar slowBar, ref GuiBar fastBar, ref TMP_Text barText)
            {
                fastBar.SetMaxValue(max);
                fastBar.SetValue(current);
                slowBar.SetMaxValue(max);
                slowBar.SetValue(current);

                barText.fontSize = Main.customBarTextSize.Value;
                barText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
            }

            public static void StaminaStyleUpdate(float max, float current, ref GuiBar slowBar, ref GuiBar fastBar, ref TMP_Text barText)
            {
                fastBar.SetValue(current / max);
                slowBar.SetValue(current / max);

                barText.fontSize = Main.customBarTextSize.Value;
                barText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
            }

            public static CustomBarState IncrementRotation(CustomBarState state)
            {
                switch (state)
                {
                    case CustomBarState.on0Degrees:
                        return CustomBarState.on90Degrees;

                    case CustomBarState.on90Degrees:
                        return CustomBarState.on180Degrees;

                    case CustomBarState.on180Degrees:
                        return CustomBarState.on270Degrees;

                    case CustomBarState.on270Degrees:
                        return CustomBarState.on0Degrees;

                    default:
                        return state;
                }
            }

            public static CustomBarState DecrementRotation(CustomBarState state)
            {
                switch (state)
                {
                    case CustomBarState.on0Degrees:
                        return CustomBarState.on270Degrees;

                    case CustomBarState.on90Degrees:
                        return CustomBarState.on0Degrees;

                    case CustomBarState.on180Degrees:
                        return CustomBarState.on0Degrees;

                    case CustomBarState.on270Degrees:
                        return CustomBarState.on180Degrees;

                    default:
                        return state;
                }
            }
        }

        public static class HealthBar
        {
            public const string objectName = "BetterUI_HPBar";
            internal static RectTransform root;
            internal static GuiBar slowBar;
            internal static GuiBar fastBar;
            internal static TMP_Text barText;

            public static void UpdateRotation()
            {
                if (root == null || Main.customHealthBar.Value == Main.CustomBarState.off)
                {
                    return;
                }

                BarHelper.UpdateRotation((int)Main.customHealthBar.Value, ref root, ref barText);
            }

            public static void Create()
            {
                try
                {
                    BarHelper.BaseCreate(objectName, "healthpanel", ref root, ref slowBar, ref fastBar, ref barText);
                    UpdateRotation();

                    // go to a good default position that can get overriden by the editing feature if needed
                    // go up and left
                    root.position += new Vector3(-BarHelper.StepSize / 4, BarHelper.StepSize / 4);
                }
                catch (Exception e)
                {
                    Debug.LogError($"{nameof(HealthBar)}.{nameof(Create)}() {e.Message} {e.StackTrace}");
                }
            }

            public static void Update(float max, float current)
            {
                try
                {
                    if (root != null)
                    {
                        BarHelper.HealthStyleUpdate(max, current, ref slowBar, ref fastBar, ref barText);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"{nameof(HealthBar)}.{nameof(Update)}() {e.Message} {e.StackTrace}");
                }
            }
        }

        public static class StaminaBar
        {
            public const string objectName = "BetterUI_StaminaBar";
            internal static RectTransform root;
            internal static GuiBar slowBar;
            internal static GuiBar fastBar;
            internal static TMP_Text barText;

            public static void UpdateRotation()
            {
                if (root == null || Main.customStaminaBar.Value == Main.CustomBarState.off)
                {
                    return;
                }

                BarHelper.UpdateRotation((int)Main.customStaminaBar.Value, ref root, ref barText);
            }

            public static void Create()
            {
                try
                {
                    BarHelper.BaseCreate(objectName, "staminapanel", ref root, ref slowBar, ref fastBar, ref barText);
                    UpdateRotation();

                    fastBar.m_originalColor = Hud.instance.m_staminaBar2Fast.m_bar.GetComponent<Image>().color;
                    slowBar.m_originalColor = Hud.instance.m_staminaBar2Slow.m_bar.GetComponent<Image>().color;
                    fastBar.ResetColor();
                    slowBar.ResetColor();

                    fastBar.m_smoothDrain = Hud.instance.m_staminaBar2Fast.m_smoothDrain;
                    fastBar.m_changeDelay = Hud.instance.m_staminaBar2Fast.m_changeDelay;
                    fastBar.m_smoothSpeed = Hud.instance.m_staminaBar2Fast.m_smoothSpeed;

                    // go to a good default position that can get overriden by the editing feature if needed
                    // go left
                    root.position -= new Vector3(BarHelper.StepSize / 4, 0);

                    if (Main.customHealthBar.Value != Main.CustomBarState.off)
                    {
                        // hold positon
                        root.position -= new Vector3(0, BarHelper.padding);
                    }
                    else
                    {
                        // go up
                        root.position += new Vector3(0, BarHelper.StepSize / 4);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"{nameof(StaminaBar)}.{nameof(Create)}() {e.Message} {e.StackTrace}");
                }
            }

            public static void Update(float max, float current)
            {
                try
                {
                    if (root != null)
                    {
                        BarHelper.StaminaStyleUpdate(max, current, ref slowBar, ref fastBar, ref barText);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"{nameof(StaminaBar)}.{nameof(Update)}() {e.Message} {e.StackTrace}");
                }
            }
        }

        public static class EitrBar
        {
            public const string objectName = "BetterUI_EitrBar";
            internal static RectTransform root;
            internal static GuiBar slowBar;
            internal static GuiBar fastBar;
            internal static TMP_Text barText;

            public static void UpdateRotation()
            {
                if (root == null || Main.customEitrBar.Value == Main.CustomBarState.off)
                {
                    return;
                }

                BarHelper.UpdateRotation((int)Main.customEitrBar.Value, ref root, ref barText);
            }

            public static void Create()
            {
                try
                {
                    BarHelper.BaseCreate("BetterUI_EitrBar", "eitrpanel", ref root, ref slowBar, ref fastBar, ref barText);
                    UpdateRotation();

                    fastBar.m_originalColor = Hud.instance.m_eitrBarFast.m_bar.GetComponent<Image>().color;
                    slowBar.m_originalColor = Hud.instance.m_eitrBarSlow.m_bar.GetComponent<Image>().color;
                    fastBar.ResetColor();
                    slowBar.ResetColor();

                    fastBar.m_smoothDrain = Hud.instance.m_eitrBarFast.m_smoothDrain;
                    fastBar.m_changeDelay = Hud.instance.m_eitrBarFast.m_changeDelay;
                    fastBar.m_smoothSpeed = Hud.instance.m_eitrBarFast.m_smoothSpeed;

                    // go to a good default position that can get overriden by the editing feature if needed
                    // go left
                    root.position -= new Vector3(BarHelper.StepSize / 4, 0);

                    if (Main.customHealthBar.Value != Main.CustomBarState.off)
                    {
                        // hold position
                        root.position -= new Vector3(0, BarHelper.padding);

                        if (Main.customStaminaBar.Value != Main.CustomBarState.off)
                        {
                            // go down
                            root.position -= new Vector3(0, BarHelper.StepSize / 4 + BarHelper.padding);
                        }
                    }
                    else
                    {
                        // go up
                        root.position += new Vector3(0, BarHelper.StepSize / 4);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"{nameof(EitrBar)}.{nameof(Create)}() {e.Message} {e.StackTrace}");
                }
            }

            public static void Update(float max, float current)
            {
                try
                {
                    if (root != null)
                    {
                        BarHelper.StaminaStyleUpdate(max, current, ref slowBar, ref fastBar, ref barText);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"{nameof(EitrBar)}.{nameof(Update)}() {e.Message} {e.StackTrace}");
                }
            }
        }

        public static class FoodBar
        {
            public const string objectName = "BetterUI_FoodBar";
            private static RectTransform foodPanel;
            private static RectTransform foodBarRoot;
            private static RectTransform foodBaseBar;

            private static Image[] foodBars;
            private static Image[] foodIcons;
            private static TMP_Text[] foodTimes;
            private static Transform[] foodTransforms;

            public static void UpdateRotation()
            {
                if (foodPanel == null || Main.customFoodBar.Value == Main.CustomBarState.off)
                {
                    return;
                }

                int rot = ((int)Main.customFoodBar.Value + 270) % 360;
                foodPanel.localEulerAngles = new Vector3(0, 0, rot);

                foreach (var item in foodTransforms)
                {
                    item.localEulerAngles = new Vector3(0, 0, -rot);
                }
            }

            public static void Create()
            {
                try
                {
                    // we've obviously already done this before if it's not null
                    if (foodPanel != null)
                    {
                        return;
                    }

                    // original food panel gets hidden by hiding the original health bar, so if the user doesn't use that feature, then they would have two food bars
                    if (Main.customHealthBar.Value == Main.CustomBarState.off)
                    {
                        Debug.LogWarning($"{nameof(Main.customFoodBar)} requires {nameof(Main.customHealthBar)}. No custom food bar will be created. Activate {nameof(Main.customHealthBar)} and log out and back in to use {nameof(Main.customFoodBar)}.");
                        return;
                    }

                    foodPanel = UnityEngine.Object.Instantiate(Hud.instance.m_healthPanel, Hud.instance.transform.Find("hudroot"));
                    foodPanel.gameObject.name = objectName;
                    foodPanel.gameObject.SetActive(true);

                    foodBarRoot = foodPanel.Find("Food").GetComponent<RectTransform>();
                    foodBaseBar = foodBarRoot.Find("baseBar").GetComponent<RectTransform>();

                    foodBars = new Image[Hud.instance.m_foodBars.Length];
                    foodIcons = new Image[Hud.instance.m_foodIcons.Length];
                    foodTimes = new TMP_Text[Hud.instance.m_foodTime.Length];
                    foodTransforms = new Transform[Hud.instance.m_foodTime.Length];

                    for (int i = 0; i < Hud.instance.m_foodBars.Length; i++)
                    {
                        foodBars[i] = foodBarRoot.Find(Hud.instance.m_foodBars[i].name).GetComponent<Image>();
                        foodTransforms[i] = foodPanel.Find($"food{i}");
                        foodIcons[i] = foodTransforms[i].Find($"foodicon{i}").GetComponent<Image>();
                        foodTimes[i] = foodTransforms[i].Find($"time").GetComponent<TMP_Text>();
                    }

                    UpdateRotation();

                    // Stuff to remove / hide
                    foodPanel.Find("Health").gameObject.SetActive(false);
                    foodPanel.Find("darken").gameObject.SetActive(false);
                    foodPanel.Find("healthicon").gameObject.SetActive(false);

                    // hide the fork icon. mistlands renamed this one, probably from editor dropping a new version into the scene (might also explain why the armor icon had a fork icon)
                    foodPanel.Find("foodicon (1)").gameObject.SetActive(false);

                    foodPanel.position = Hud.instance.m_gpRoot.position;
                    foodPanel.position += new Vector3(-foodPanel.sizeDelta.x / 4, foodPanel.sizeDelta.x / 2);
                }
                catch (Exception e)
                {
                    Debug.LogError($"{nameof(FoodBar)}.{nameof(Create)}() {e.Message} {e.StackTrace}");
                }
            }

            // based on HUD.UpdateFood()
            public static void Update(Player player)
            {
                try
                {
                    if (foodPanel != null)
                    {
                        List<Player.Food> foods = player.GetFoods();
                        float baseHP = player.GetBaseFoodHP() / 25f * 32f;
                        foodBaseBar.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, baseHP);
                        float barLength = baseHP;

                        for (int i = 0; i < Hud.instance.m_foodBars.Length; i++)
                        {
                            Image foodBar = foodBars[i];
                            Image foodIcon = foodIcons[i];
                            TMP_Text foodTime = foodTimes[i];

                            if (i < foods.Count)
                            {
                                foodBar.gameObject.SetActive(true);
                                Player.Food food = foods[i];
                                foodIcon.gameObject.SetActive(true);
                                foodIcon.sprite = food.m_item.GetIcon();

                                if (food.CanEatAgain())
                                {
                                    foodIcon.color = new Color(1f, 1f, 1f, 0.7f + Mathf.Sin(Time.time * 5f) * 0.3f);
                                }
                                else
                                {
                                    foodIcon.color = Color.white;
                                }

                                foodTime.gameObject.SetActive(true);
                                foodTime.fontSize = customFoodBarTextSize.Value;

                                if (food.m_time >= 60f)
                                {
                                    foodTime.text = Mathf.CeilToInt(food.m_time / 60f) + "m";
                                    foodTime.color = Color.white;
                                }
                                else
                                {
                                    foodTime.text = Mathf.FloorToInt(food.m_time) + "s";
                                    foodTime.color = new Color(1f, 1f, 1f, 0.4f + Mathf.Sin(Time.time * 10f) * 0.6f);
                                }
                            }
                            else
                            {
                                foodBar.gameObject.SetActive(false);
                                foodIcon.gameObject.SetActive(false);
                                foodTime.gameObject.SetActive(false);
                            }
                        }

                        float size = Mathf.Ceil(player.GetMaxHealth() / 25f * 32f);
                        foodBarRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"{nameof(FoodBar)}.{nameof(Update)}() {e.Message} {e.StackTrace}");
                }
            }
        }
    }
}