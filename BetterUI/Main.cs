﻿using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BetterUI.GameClasses;

using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace BetterUI
{
    [BepInPlugin(GUID, MODNAME, VERSION)]
    public class Main : BaseUnityPlugin
    {
        #region[Declarations]

        public const string
          MODNAME = "BetterUI",
          AUTHOR = "MK",
          GUID = AUTHOR + "_" + MODNAME,
          VERSION = "2.5.7";

        internal static ManualLogSource log;
        internal readonly Harmony harmony;
        internal readonly Assembly assembly;

        // Player HUD
        public static ConfigEntry<bool> enablePlayerHudEditing;

        public static ConfigEntry<KeyCode> togglePlayerHudEditModeKey;
        public static ConfigEntry<KeyCode> modKeyPrimary;
        public static ConfigEntry<KeyCode> modKeySecondary;
        public static ConfigEntry<CustomBarState> customHealthBar;
        public static ConfigEntry<CustomBarState> customStaminaBar;
        public static ConfigEntry<CustomBarState> customEitrBar;
        public static ConfigEntry<CustomBarState> customFoodBar;
        public static ConfigEntry<int> customBarTextSize;
        public static ConfigEntry<int> customFoodBarTextSize;

        // Player Inventory
        public static ConfigEntry<DurabilityBarStyle> durabilityBarColorPalette;

        public static ConfigEntry<bool> showItemStars;
        public static ConfigEntry<bool> showCustomCharInfo;
        public static ConfigEntry<bool> showCustomTooltips;
        public static ConfigEntry<bool> showCombinedItemStats;
        public static ConfigEntry<float> iconScaleSize;

        // Skill UI
        public static ConfigEntry<bool> customSkillUI;

        public static ConfigEntry<int> skillUITextSize;

        // HoverTexts
        public static ConfigEntry<TimeLeftStyle> timeLeftHoverTextFermenter;

        public static ConfigEntry<TimeLeftStyle> timeLeftHoverTextPlant;
        public static ConfigEntry<TimeLeftStyle> timeLeftHoverTextCookingStation;
        public static ConfigEntry<TimeLeftStyle> timeLeftHoverTextBeeHive;
        public static ConfigEntry<ChestHasRoomStyle> chestHasRoomHoverText;

        // Character XP
        public static ConfigEntry<bool> showCharacterXP;

        public static ConfigEntry<bool> showCharacterXpBar;
        public static ConfigEntry<bool> showXPNotifications;
        public static ConfigEntry<bool> extendedXPNotification;
        public static ConfigEntry<bool> skipRunningSkillNotifications;
        public static ConfigEntry<int> notificationTextSizeXP;

        // Enemy HUD
        public static ConfigEntry<bool> customEnemyHud;

        public static ConfigEntry<bool> showEnemyHPText;
        public static ConfigEntry<EnemyLevelStyle> enemyLevelStyle;
        public static ConfigEntry<int> enemyNameTextSize;
        public static ConfigEntry<int> enemyHPTextSize;
        public static ConfigEntry<int> playerHPTextSize;
        public static ConfigEntry<bool> showPlayerHPText;
        public static ConfigEntry<bool> showLocalPlayerEnemyHud;
        public static ConfigEntry<int> bossHPTextSize;
        public static ConfigEntry<bool> makeTamedHPGreen;
        public static ConfigEntry<float> maxShowDistance;
        public static ConfigEntry<bool> useCustomAlertedStatus;

        // Map
        public static ConfigEntry<float> mapPinScaleSize;

        // xUIData
        public static ConfigEntry<string> uiData;

        // Debug
        public static ConfigEntry<bool> isDebug;

        #endregion

        public Main()
        {
            log = Logger;
            harmony = new Harmony(GUID);
            assembly = Assembly.GetExecutingAssembly();
        }

        public void Awake()
        {
            string sectionName;

            // Don't be tempted to rename the sections. That resets all its config values for every user
            // The same goes for moving items between sections
            // That's also why there is no section number counter

            //
            // Player HUD
            sectionName = "1 - Player HUD";

            togglePlayerHudEditModeKey = Config.Bind(sectionName, nameof(togglePlayerHudEditModeKey), KeyCode.F7,
                "Key used to toggle Player HUD editing mode. Accepted values: https://docs.unity3d.com/ScriptReference/KeyCode.html");

            modKeyPrimary = Config.Bind(sectionName, nameof(modKeyPrimary), KeyCode.Mouse0,
                "Key needed to be held down to change an elements position by moving the mouse, as well as its rotation with the mouse wheel if supported. Accepted values: https://docs.unity3d.com/ScriptReference/KeyCode.html");

            modKeySecondary = Config.Bind(sectionName, nameof(modKeySecondary), KeyCode.LeftControl,
                "Key needed to be held down to change an elements scale with the mouse wheel, as well as its X and Y dimensions by moving the mouse. Accepted Values: https://docs.unity3d.com/ScriptReference/KeyCode.html");

            customBarTextSize = Config.Bind(sectionName, nameof(customBarTextSize), 15, "Font size of the text on the custom bars.");
            customFoodBarTextSize = Config.Bind(sectionName, nameof(customFoodBarTextSize), 15, "Font size of the duration text of food items in the custom food bar.");

            // Player HUD RESTART
            sectionName = "1 - Player HUD (Requires Logout)";

            bool editingDefault = GetOldOrDefaultConfigValue(new ConfigDefinition("1 - Player HUD", nameof(enablePlayerHudEditing)), true);
            enablePlayerHudEditing = Config.Bind(sectionName, nameof(enablePlayerHudEditing), editingDefault, "Enable the ability to edit the player HUD by pressing a hotkey.");

            bool healthDefault = GetOldOrDefaultConfigValue(new ConfigDefinition("1 - Player HUD", "useCustomHealthBar"), false);
            healthDefault |= GetOldOrDefaultConfigValue(new ConfigDefinition("1 - Player HUD (Requires Logout)", "useCustomHealthBar"), false);
            customHealthBar = Config.Bind(sectionName, nameof(customHealthBar), healthDefault ? CustomBarState.on0Degrees : CustomBarState.off, $"Resizable, rotatable HP bar. This bar will always be the same size and will not get longer when you eat. Will also disable the default food bar, so {nameof(customFoodBar)} will be enabled automatically.");
            customHealthBar.SettingChanged += (_, _) => CustomHealthBar_SettingChanged();
            RemoveOldConfigValue<int>(new ConfigDefinition("1 - Player HUD", "healthBarRotation"));
            RemoveOldConfigValue<int>(new ConfigDefinition(sectionName, "customHealthBarRotation"));

            bool staminaDefault = GetOldOrDefaultConfigValue(new ConfigDefinition("1 - Player HUD", "useCustomStaminaBar"), false);
            staminaDefault |= GetOldOrDefaultConfigValue(new ConfigDefinition(sectionName, "useCustomStaminaBar"), false);
            customStaminaBar = Config.Bind(sectionName, nameof(customStaminaBar), staminaDefault ? CustomBarState.on0Degrees : CustomBarState.off, "Resizable, rotatable stamina bar. This bar will always be visible and will not get longer when you eat.");
            customStaminaBar.SettingChanged += (_, _) => CustomStaminaBar_SettingChanged();
            RemoveOldConfigValue<int>(new ConfigDefinition("1 - Player HUD", "staminaBarRotation"));
            RemoveOldConfigValue<int>(new ConfigDefinition(sectionName, "customStaminaBarRotation"));

            bool foodDefault = GetOldOrDefaultConfigValue(new ConfigDefinition("1 - Player HUD", "useCustomFoodBar"), false);
            foodDefault |= GetOldOrDefaultConfigValue(new ConfigDefinition(sectionName, "useCustomFoodBar"), false);
            customFoodBar = Config.Bind(sectionName, nameof(customFoodBar), foodDefault ? CustomBarState.on0Degrees : CustomBarState.off, $"Resizable, rotatable food bar. Requires {nameof(customHealthBar)}.");
            // if the customHealthBar is enabled the vanilla food bar is removed. If the customeFoodBAr is disabled, enable it so the user doesn't end up with no food bar.
            if (customHealthBar.Value != CustomBarState.off && customFoodBar.Value == CustomBarState.off)
            {
                customFoodBar.Value = CustomBarState.on0Degrees;
            }
            customFoodBar.SettingChanged += (_, _) => CustomFoodBar_SettingChanged();
            RemoveOldConfigValue<int>(new ConfigDefinition("1 - Player HUD", "foodBarRotation"));
            RemoveOldConfigValue<int>(new ConfigDefinition(sectionName, "customFoodBarRotation"));

            bool eitrDefault = GetOldOrDefaultConfigValue(new ConfigDefinition("1 - Player HUD", "useCustomEitrBar"), false);
            customEitrBar = Config.Bind(sectionName, nameof(customEitrBar), eitrDefault ? CustomBarState.on0Degrees : CustomBarState.off, "Resizable, rotatable eitr bar. If you don't know what this is yet, just keep it disabled. This bar will always be visible and will not get longer when you eat.");
            customEitrBar.SettingChanged += (_, _) => CustomEitrBar_SettingChanged();
            RemoveOldConfigValue<int>(new ConfigDefinition(sectionName, "customSpoilerBarRotation"));

            //
            // Character Inventory
            sectionName = "2 - Character Inventory";

            bool wasColorOnDefault = GetOldOrDefaultConfigValue(new ConfigDefinition(sectionName, "showDurabilityColor"), true);
            int colorDefault = GetOldOrDefaultConfigValue(new ConfigDefinition(sectionName, "durabilityColorPalette"), 0);
            durabilityBarColorPalette = Config.Bind(sectionName, nameof(durabilityBarColorPalette), IntToDurabilityBarStyle(wasColorOnDefault, colorDefault), "Change durability bar colors. Options: 0 = Green, Yellow, Orange, Red, 1 = White, Light Yellow, Light Cyan, Blue.");

            showItemStars = Config.Bind(sectionName, nameof(showItemStars), true, "Show item quality as stars.");

            showCustomCharInfo = Config.Bind(sectionName, nameof(showCustomCharInfo), true, "Show Deaths, Builds, and Crafts stats on character selection screen. Also shows the Kills stat if something increases it (the base game doesn't).");

            showCustomTooltips = Config.Bind(sectionName, nameof(showCustomTooltips), true, "Show more info on inventory item tooltips. Automatically disabled this if using Epic Loot for compatibility.");

            showCombinedItemStats = Config.Bind(sectionName, nameof(showCombinedItemStats), true, "Show all item stats when mouse is hovered over armor amount.");

            iconScaleSize = Config.Bind(sectionName, nameof(iconScaleSize), 1f, "Scale item icon by this factor. Ex. 0.75 makes them 75% of their original size.");

            //
            // Skills UI
            sectionName = "3 - Character Skills";

            customSkillUI = Config.Bind(sectionName, nameof(customSkillUI), false, "Toggle the use of the custom skills UI.");

            skillUITextSize = Config.Bind(sectionName, nameof(skillUITextSize), 14, "Select text size of the skills UI.");

            //
            // Hover Text
            sectionName = "4 - Hover Text";
            int timeLeftDefault;

            timeLeftDefault = GetOldOrDefaultConfigValue(new ConfigDefinition(sectionName, "timeLeftStyleFermenter"), 2);
            timeLeftHoverTextFermenter = Config.Bind(sectionName, nameof(timeLeftHoverTextFermenter), IntToTimeLeftStyle(timeLeftDefault), "Select duration display. Disabled = Default, PercentageDone = % Done, MinutesSecondsLeft = min:sec left.");

            timeLeftDefault = GetOldOrDefaultConfigValue(new ConfigDefinition(sectionName, "timeLeftStylePlant"), 2);
            timeLeftHoverTextPlant = Config.Bind(sectionName, nameof(timeLeftHoverTextPlant), IntToTimeLeftStyle(timeLeftDefault), "Select duration display. Disabled = Default, PercentageDone = % Done, MinutesSecondsLeft = min:sec left.");

            timeLeftDefault = GetOldOrDefaultConfigValue(new ConfigDefinition(sectionName, "timeLeftStyleCookingStation"), 2);
            timeLeftHoverTextCookingStation = Config.Bind(sectionName, nameof(timeLeftHoverTextCookingStation), IntToTimeLeftStyle(timeLeftDefault), "Select duration display. Disabled = Default, PercentageDone = % Done, MinutesSecondsLeft = min:sec left.");

            timeLeftHoverTextBeeHive = Config.Bind(sectionName, nameof(timeLeftHoverTextBeeHive), TimeLeftStyle.MinutesSecondsLeft, "Select duration display. Disabled = Default, PercentageDone = % Done, MinutesSecondsLeft = min:sec left.");

            int chestStyleDefault = GetOldOrDefaultConfigValue(new ConfigDefinition(sectionName, "chestHasRoomStyle"), 2);
            chestHasRoomHoverText = Config.Bind(sectionName, nameof(chestHasRoomHoverText), IntToChestHasRoomStyle(chestStyleDefault), "Select how chest emptiness is displayed. Disabled = Default | Percentage = % | ItemsSlashMaxRoom= used / total slots. | AmountOfFreeSlots = count of free slots.");

            //
            // Character XP
            sectionName = "5 - Character XP";

            showCharacterXP = Config.Bind(sectionName, nameof(showCharacterXP), true, "Enable character XP. This combines all skill levels to show overall character progress.");

            showXPNotifications = Config.Bind(sectionName, nameof(showXPNotifications), true, "Show whenever you gain xp from actions.");

            notificationTextSizeXP = Config.Bind(sectionName, nameof(notificationTextSizeXP), 14, "XP notification font size.");

            extendedXPNotification = Config.Bind(sectionName, nameof(extendedXPNotification), false, "Extend notification with: (xp gained) [current/overall xp].");

            skipRunningSkillNotifications = Config.Bind(sectionName, nameof(skipRunningSkillNotifications), true, "Whether to ignore xp gain notifications for the running skill.");

            // Character XP RESTART
            sectionName = "5 - Character XP (Requires Logout)";

            bool expBarDefault = GetOldOrDefaultConfigValue(new ConfigDefinition("5 - Character XP", nameof(showCharacterXpBar)), true);
            showCharacterXpBar = Config.Bind(sectionName, nameof(showCharacterXpBar), expBarDefault, "Show Character XP bar on the bottom of the screen. Character XP must be enabled.");

            //
            // Enemy HUD
            sectionName = "6 - Enemy HUD";

            customEnemyHud = Config.Bind(sectionName, nameof(customEnemyHud), true, "Enable custom enemy HUD changes. If this is set to false, all options in this section will be disabled.");

            useCustomAlertedStatus = Config.Bind(sectionName, nameof(useCustomAlertedStatus), true, "Hide the vanilla alerted icons above the enemy health bar and instead change the color of the name based on the alerted status.");

            showEnemyHPText = Config.Bind(sectionName, nameof(showEnemyHPText), true, "Show the text with HP amount on enemy health bars.");

            int enemyLevelStyleDefault = GetOldOrDefaultConfigValue(new ConfigDefinition(sectionName, "enemyLvlStyle"), 0);
            enemyLevelStyle = Config.Bind(sectionName, nameof(enemyLevelStyle), IntToEnemyLevelStyle(enemyLevelStyleDefault), "Choose how enemy level is shown.");

            enemyNameTextSize = Config.Bind(sectionName, nameof(enemyNameTextSize), 14, "Font size of the name on the enemy.");

            enemyHPTextSize = Config.Bind(sectionName, nameof(enemyHPTextSize), 10, "Font size of the HP text on the enemy health bar.");

            showPlayerHPText = Config.Bind(sectionName, nameof(showPlayerHPText), true, "Show the health numbers on other player's health bar in multiplayer.");

            showLocalPlayerEnemyHud = Config.Bind(sectionName, nameof(showLocalPlayerEnemyHud), false, "Show the enemy HUD/ health Bar for your player.");
            showLocalPlayerEnemyHud.SettingChanged += (_, _) => BetterEnemyHud.ShowLocalPlayerEnemyHudConfigChanged();

            playerHPTextSize = Config.Bind(sectionName, nameof(playerHPTextSize), 10, "The size of the font to display on other player's health bar in multiplayer.");

            bossHPTextSize = Config.Bind(sectionName, nameof(bossHPTextSize), 14, "The size of the font to display on the boss's health bar.");

            makeTamedHPGreen = Config.Bind(sectionName, nameof(makeTamedHPGreen), true, "Make the health bar for tamed creatures green instead of red.");

            maxShowDistance = Config.Bind(sectionName, nameof(maxShowDistance), 1f, "How far you will see enemy HP Bar. This is a multiplier, 1 is game default, 2 is twice as far (valid range: 0 to 3).");

            //
            // Map
            sectionName = "7 - Map";

            mapPinScaleSize = Config.Bind(sectionName, nameof(mapPinScaleSize), 1f, "Scale map pins by this factor. Ex. 1.5 makes them 150% of their original size.");

            //
            // Debug
            sectionName = "8 - Debug";

            isDebug = Config.Bind(sectionName, nameof(isDebug), false, "Enable debug logging.");

            //
            // xDataUI
            sectionName = "9 - xDataUI";
            uiData = Config.Bind(sectionName, nameof(uiData), "none", "This is your customized UI info. Edit to none, if having issues or wanting to reset positions.");

            if (isDebug.Value)
            {
                PrintOrphanedEntries();
            }
            
            this.Logger.LogInfo("BetterUI (Forever Maintained Version) loaded");
        }

        private DurabilityBarStyle IntToDurabilityBarStyle(bool wasColorPaletteOn, int selectedColorPalette)
        {
            if (!wasColorPaletteOn)
            {
                return DurabilityBarStyle.Disabled;
            }
            else
            {
                return selectedColorPalette == 0 ? DurabilityBarStyle.GreenYellowOrangeRed : DurabilityBarStyle.WhiteLightYellowLightCyanBlue;
            }
        }

        private ChestHasRoomStyle IntToChestHasRoomStyle(int value)
        {
            if (value > 0 && value <= 3)
            {
                return (ChestHasRoomStyle)value;
            }
            else
            {
                return 0;
            }
        }

        private TimeLeftStyle IntToTimeLeftStyle(int value)
        {
            switch (value)
            {
                case 1:
                    return TimeLeftStyle.PercentageDone;

                case 2:
                    return TimeLeftStyle.MinutesSecondsLeft;

                default:
                    return TimeLeftStyle.Disabled;
            }
        }

        private EnemyLevelStyle IntToEnemyLevelStyle(int value)
        {
            switch (value)
            {
                case 1:
                    return EnemyLevelStyle.PrefixLevelNumber;

                case 2:
                    return EnemyLevelStyle.Both;

                default:
                    return EnemyLevelStyle.DefaultStars;
            }
        }

        private void CustomFoodBar_SettingChanged()
        {
            Patches.CustomBars.FoodBar.UpdateRotation();
        }

        private void CustomStaminaBar_SettingChanged()
        {
            Patches.CustomBars.StaminaBar.UpdateRotation();
        }

        private void CustomHealthBar_SettingChanged()
        {
            Patches.CustomBars.HealthBar.UpdateRotation();
        }

        private void CustomEitrBar_SettingChanged()
        {
            Patches.CustomBars.EitrBar.UpdateRotation();
        }

        public void Start()
        {
            harmony.PatchAll(assembly);
        }

        public void RemoveOldConfigValue<T>(ConfigDefinition configDefinition)
        {
            GetOldOrDefaultConfigValue(configDefinition, default(T));
        }

        /// <summary>
        /// Rebinds the old orphan config definition and then immediately removes it again, and returns its last value
        /// This properly deletes an old config value
        /// </summary>
        public T GetOldOrDefaultConfigValue<T>(ConfigDefinition configDefinition, T defaultValue)
        {
            T oldOrDefaultValue = Config.Bind(configDefinition, defaultValue).Value;
            Config.Remove(configDefinition);

            if (Config.SaveOnConfigSet)
            {
                Config.Save();
            }

            return oldOrDefaultValue;
        }

        public bool TryGetOldConfigValue<T>(ConfigDefinition configDefinition, ref T oldValue, bool removeIfFound = true)
        {
            if (!TomlTypeConverter.CanConvert(typeof(T)))
            {
                throw new ArgumentException($"Type {typeof(T)} is not supported by the config system. Supported types: {string.Join(", ", (from x in TomlTypeConverter.GetSupportedTypes() select x.Name).ToArray())}");
            }

            try
            {
                object iolock = AccessTools.FieldRefAccess<ConfigFile, object>("_ioLock").Invoke(Config);
                Dictionary<ConfigDefinition, string> orphanedEntries = (Dictionary<ConfigDefinition, string>)AccessTools.PropertyGetter(typeof(ConfigFile), "OrphanedEntries").Invoke(Config, new object[0]);

                lock (iolock)
                {
                    if (orphanedEntries.TryGetValue(configDefinition, out string oldValueString))
                    {
                        oldValue = (T)TomlTypeConverter.ConvertToValue(oldValueString, typeof(T));

                        if (removeIfFound)
                        {
                            orphanedEntries.Remove(configDefinition);
                        }

                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                BetterUI.Main.log.LogWarning($"Error getting orphaned entry: {e.StackTrace}");
            }

            return false;
        }

        public void PrintOrphanedEntries()
        {
            try
            {
                object iolock = AccessTools.FieldRefAccess<ConfigFile, object>("_ioLock").Invoke(Config);
                Dictionary<ConfigDefinition, string> orphanedEntries = (Dictionary<ConfigDefinition, string>)AccessTools.PropertyGetter(typeof(ConfigFile), "OrphanedEntries").Invoke(Config, new object[0]);

                if (orphanedEntries.Count == 0)
                {
                    return;
                }

                lock (iolock)
                {
                    BetterUI.Main.log.LogInfo("printing orphaned config values");

                    foreach (KeyValuePair<ConfigDefinition, string> item in orphanedEntries)
                    {
                        BetterUI.Main.log.LogInfo($"{item.Key.Section},{item.Key.Key}: {item.Value}");
                    }
                }
            }
            catch (Exception e)
            {
                BetterUI.Main.log.LogWarning($"Error logging orphaned entries: {e.StackTrace}");
            }
        }

        public enum CustomBarState
        {
            off = -1,
            on0Degrees = 0,
            on90Degrees = 90,
            on180Degrees = 180,
            on270Degrees = 270
        }

        public enum DurabilityBarStyle
        {
            Disabled = -1,
            GreenYellowOrangeRed = 0,
            WhiteLightYellowLightCyanBlue = 1
        }

        public enum TimeLeftStyle
        {
            Disabled = 0,
            PercentageDone = 1,
            MinutesSecondsLeft = 2
        }

        public enum ChestHasRoomStyle
        {
            Disabled = 0,
            Percentage = 1,
            ItemsSlashMaxRoom = 2,
            AmountOfFreeSlots = 3
        }

        public enum EnemyLevelStyle
        {
            DefaultStars = 0,
            PrefixLevelNumber = 1,
            Both = 2
        }
    }
}