using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BetterUI.Patches
{
    internal class CustomElements
    {
        public static class BarHelper
        {
            public static float StepSize => Hud.instance.m_healthPanel.sizeDelta.x;

            public const float scalingFactor = 0.6f;
            public const int padding = 0;

            public static void BaseCreate(string objectName, string origBarName, int configRotation, ref RectTransform root, ref GuiBar slowBar, ref GuiBar fastBar, ref Text barText)
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

                int rot = 90 - (configRotation / 90 % 4 * 90);
                root.localEulerAngles = new Vector3(0, 0, rot);

                fastBar = root.Find("fast").GetComponent<GuiBar>();
                slowBar = root.Find("slow").GetComponent<GuiBar>();

                root.Find("fast").Find("bar").Find("HealthText").gameObject.SetActive(false);

                barText = UnityEngine.Object.Instantiate(root.Find("fast").Find("bar").Find("HealthText").GetComponent<Text>(), root);
                barText.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, -rot);

                // Resize to a more "slim" rectangle - authors preference
                barText.GetComponent<RectTransform>().localScale = new Vector3(scalingFactor, scalingFactor, 1f);
                barText.gameObject.SetActive(true);

                root.Find("border").GetComponent<RectTransform>().localScale = new Vector3(1f, scalingFactor, 1f);
                root.Find("bkg").GetComponent<RectTransform>().localScale = new Vector3(1f, scalingFactor, 1f);

                fastBar.GetComponent<RectTransform>().localScale = new Vector3(1f, scalingFactor, 1f);
                slowBar.GetComponent<RectTransform>().localScale = new Vector3(1f, scalingFactor, 1f);
            }

            public static void HealthStyleUpdate(float max, float current, ref GuiBar slowBar, ref GuiBar fastBar, ref Text barText)
            {
                fastBar.SetMaxValue(max);
                fastBar.SetValue(current);
                slowBar.SetMaxValue(max);
                slowBar.SetValue(current);

                barText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
            }

            public static void StaminaStyleUpdate(float max, float current, ref GuiBar slowBar, ref GuiBar fastBar, ref Text barText)
            {
                fastBar.SetValue(current / max);
                slowBar.SetValue(current / max);

                barText.text = $"{Mathf.CeilToInt(current)}/{Mathf.CeilToInt(max)}";
            }
        }

        public static class HealthBar
        {
            public static readonly string objectName = "BetterUI_HPBar";
            internal static RectTransform root;
            internal static GuiBar slowBar;
            internal static GuiBar fastBar;
            internal static Text barText;

            public static void Create()
            {
                try
                {
                    BarHelper.BaseCreate(objectName, "healthpanel", Main.customHealthBarRotation.Value, ref root, ref slowBar, ref fastBar, ref barText);

                    // go to a good default position that can get overriden by the editing feature if needed
                    // go up and left
                    root.position += new Vector3(-BarHelper.StepSize / 4, BarHelper.StepSize / 4);
                }
                catch (Exception e)
                {
                    Debug.LogError($"{nameof(HealthBar)}.{nameof(Create)}() {e.Message}");
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
                    Debug.LogError($"{nameof(HealthBar)}.{nameof(Update)}() {e.Message}");
                }
            }
        }

        public static class StaminaBar
        {
            public static readonly string objectName = "BetterUI_StaminaBar";
            internal static RectTransform root;
            internal static GuiBar slowBar;
            internal static GuiBar fastBar;
            internal static Text barText;

            public static void Create()
            {
                try
                {
                    BarHelper.BaseCreate(objectName, "staminapanel", Main.customStaminaBarRotation.Value, ref root, ref slowBar, ref fastBar, ref barText);

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

                    if (Main.useCustomHealthBar.Value)
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
                    Debug.LogError($"{nameof(StaminaBar)}.{nameof(Create)}() {e.Message}");
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
                    Debug.LogError($"{nameof(StaminaBar)}.{nameof(Update)}() {e.Message}");
                }
            }
        }

        public static class EitrBar
        {
            public static readonly string objectName = "BetterUI_EitrBar";
            internal static RectTransform root;
            internal static GuiBar slowBar;
            internal static GuiBar fastBar;
            internal static Text barText;

            public static void Create()
            {
                try
                {
                    BarHelper.BaseCreate("BetterUI_EitrBar", "eitrpanel", Main.customEitrBarRotation.Value, ref root, ref slowBar, ref fastBar, ref barText);

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

                    if (Main.useCustomHealthBar.Value)
                    {
                        // hold position
                        root.position -= new Vector3(0, BarHelper.padding);

                        if (Main.useCustomStaminaBar.Value)
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
                    Debug.LogError($"{nameof(EitrBar)}.{nameof(Create)}() {e.Message}");
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
                    Debug.LogError($"{nameof(EitrBar)}.{nameof(Update)}() {e.Message}");
                }
            }
        }

        public static class FoodBar
        {
            public static readonly string objectName = "BetterUI_FoodBar";
            private static RectTransform foodPanel;
            private static RectTransform foodBarRoot;
            public static RectTransform food0;
            public static RectTransform food1;
            public static RectTransform food2;

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
                    if (!Main.useCustomHealthBar.Value)
                    {
                        Debug.LogWarning($"{nameof(Main.useCustomFoodBar)} requires {nameof(Main.useCustomHealthBar)}. No custom food bar will be created. Activate {nameof(Main.useCustomHealthBar)} and log out and back in to use {nameof(Main.useCustomFoodBar)}.");
                        return;
                    }

                    foodPanel = UnityEngine.Object.Instantiate(Hud.instance.m_healthPanel, Hud.instance.transform.Find("hudroot"));
                    foodPanel.gameObject.name = objectName;
                    foodPanel.gameObject.SetActive(true);
                    int rot = 90 - (Main.customFoodBarRotation.Value / 90 % 4 * 90);
                    foodPanel.localEulerAngles = new Vector3(0, 0, rot);

                    foodBarRoot = foodPanel.Find("Food").GetComponent<RectTransform>();
                    food0 = foodPanel.Find("food0").GetComponent<RectTransform>();
                    food1 = foodPanel.Find("food1").GetComponent<RectTransform>();
                    food2 = foodPanel.Find("food2").GetComponent<RectTransform>();

                    food0.localEulerAngles = new Vector3(0, 0, -rot);
                    food1.localEulerAngles = new Vector3(0, 0, -rot);
                    food2.localEulerAngles = new Vector3(0, 0, -rot);

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
                    Debug.LogError($"{nameof(FoodBar)}.{nameof(Create)}() {e.Message}");
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
                        foodBarRoot.Find("baseBar").GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, baseHP);
                        float barLength = baseHP;

                        for (int i = 0; i < Hud.instance.m_foodBars.Length; i++)
                        {
                            Image image = foodBarRoot.Find(Hud.instance.m_foodBars[i].name).GetComponent<Image>();
                            Image image2 = foodPanel.Find($"food{i}").Find($"foodicon{i}").GetComponent<Image>();
                            Text text = foodPanel.Find($"food{i}").Find($"time").GetComponent<Text>();

                            if (i < foods.Count)
                            {
                                image.gameObject.SetActive(true);
                                Player.Food food = foods[i];
                                image2.gameObject.SetActive(true);
                                image2.sprite = food.m_item.GetIcon();

                                if (food.CanEatAgain())
                                {
                                    image2.color = new Color(1f, 1f, 1f, 0.7f + Mathf.Sin(Time.time * 5f) * 0.3f);
                                }
                                else
                                {
                                    image2.color = Color.white;
                                }

                                text.gameObject.SetActive(true);

                                if (food.m_time >= 60f)
                                {
                                    text.text = Mathf.CeilToInt(food.m_time / 60f) + "m";
                                    text.color = Color.white;
                                }
                                else
                                {
                                    text.text = Mathf.FloorToInt(food.m_time) + "s";
                                    text.color = new Color(1f, 1f, 1f, 0.4f + Mathf.Sin(Time.time * 10f) * 0.6f);
                                }
                            }
                            else
                            {
                                image.gameObject.SetActive(false);
                                image2.gameObject.SetActive(false);
                                text.gameObject.SetActive(false);
                            }
                        }

                        float size = Mathf.Ceil(player.GetMaxHealth() / 25f * 32f);
                        foodBarRoot.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError($"{nameof(FoodBar)}.{nameof(Update)}() {e.Message}");
                }
            }
        }
    }
}