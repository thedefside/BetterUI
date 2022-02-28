using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
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
          VERSION = "2.1.0";

        internal static ManualLogSource log;
        internal readonly Harmony harmony;
        internal readonly Assembly assembly;

        
        // Player HUD
        public static ConfigEntry<bool> enablePlayerHudEditing;
        public static ConfigEntry<KeyCode> togglePlayerHudEditModeKey;
        public static ConfigEntry<KeyCode> modKeyPrimary;
        public static ConfigEntry<KeyCode> modKeySecondary;
        public static ConfigEntry<bool> useCustomHealthBar;
        public static ConfigEntry<bool> useCustomStaminaBar;
        public static ConfigEntry<bool> useCustomFoodBar;
        public static ConfigEntry<int> healthBarRotation;
        public static ConfigEntry<int> staminaBarRotation;
        public static ConfigEntry<int> foodBarRotation;

        // Player Inventory
        public static ConfigEntry<bool> showDurabilityColor;
        public static ConfigEntry<int> durabilityColorPalette;
        public static ConfigEntry<bool> showItemStars;
        public static ConfigEntry<bool> showCustomCharInfo;
        public static ConfigEntry<bool> showCustomTooltips;
        public static ConfigEntry<bool> showCombinedItemStats;
        public static ConfigEntry<float> iconScaleSize;

        // Skill UI
        public static ConfigEntry<bool> customSkillUI;
        public static ConfigEntry<int> skillUITextSize;

        // HoverTexts
        public static ConfigEntry<int> timeLeftStyleFermenter;
        public static ConfigEntry<int> timeLeftStylePlant;
        public static ConfigEntry<int> timeLeftStyleCookingStation;
        public static ConfigEntry<int> chestHasRoomStyle;
        
        // Character XP
        public static ConfigEntry<bool> showCharacterXP;
        public static ConfigEntry<bool> showCharacterXpBar;
        public static ConfigEntry<bool> showXPNotifications;
        public static ConfigEntry<bool> extendedXPNotification;
        public static ConfigEntry<int> notificationTextSizeXP;

        // Enemy HUD
        public static ConfigEntry<bool> customEnemyHud;
        public static ConfigEntry<bool> showEnemyHPText;
        public static ConfigEntry<int> enemyLvlStyle;
        public static ConfigEntry<int> enemyNameTextSize;
        public static ConfigEntry<int> enemyHPTextSize;
        public static ConfigEntry<int> playerHPTextSize;
        public static ConfigEntry<bool> showPlayerHPText;
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
            // Player HUD
            enablePlayerHudEditing = Config.Bind("1 - Player HUD", nameof(enablePlayerHudEditing), true, "Enable the ability to edit the Player HUD by pressing a hotkey. (REQUIRES RESTART)");

            togglePlayerHudEditModeKey = Config.Bind("1 - Player HUD", nameof(togglePlayerHudEditModeKey), KeyCode.F7, "Key used to toggle Player HUD editing mode.");

            modKeyPrimary = Config.Bind("1 - Player HUD", nameof( modKeyPrimary), KeyCode.Mouse0,
              "Button needed to hold down to change HUD position. Accepted values: https://docs.unity3d.com/ScriptReference/KeyCode.html");

            modKeySecondary = Config.Bind("1 - Player HUD", nameof(modKeySecondary), KeyCode.LeftControl,
              "Button needed to hold down to change element dimensions. Accepted Values: https://docs.unity3d.com/ScriptReference/KeyCode.html");

            useCustomHealthBar = Config.Bind("1 - Player HUD", nameof(useCustomHealthBar), false, "Resizable, rotatable HP bar. This bar will always be the same size and will not scale when you eat. (REQUIRES RESTART)");
            
            useCustomStaminaBar = Config.Bind("1 - Player HUD", nameof(useCustomStaminaBar), false, "Resizable, rotatable Stamina bar. This bar will always be visible and will not scale when you eat. (REQUIRES RESTART)");
            
            useCustomFoodBar = Config.Bind("1 - Player HUD", nameof(useCustomFoodBar), false, "Resizable, rotatable Food Bar. (REQUIRES RESTART)");

            healthBarRotation = Config.Bind("1 - Player HUD", nameof(healthBarRotation), 0, "Rotate healthbar in degrees  (REQUIRES RESTART)");

            staminaBarRotation = Config.Bind("1 - Player HUD", nameof(staminaBarRotation), 90, "Rotate staminabar in degrees  (REQUIRES RESTART)");

            foodBarRotation = Config.Bind("1 - Player HUD", nameof(foodBarRotation), 90, "Rotate foodbar in degrees  (REQUIRES RESTART)");
            

            // Character Inventory
            showDurabilityColor = Config.Bind("2 - Character Inventory", nameof(showDurabilityColor), true, "Show colored durability bars for items");

            durabilityColorPalette = Config.Bind("2 - Character Inventory", nameof(durabilityColorPalette), 0, "Change Durabilty bar colors. Options: 0=Green,Yellow,Orange,Red, 1=White,Light Yellow,Light Cyan,Blue. ");

            showItemStars = Config.Bind("2 - Character Inventory", nameof(showItemStars), true, "Show item quality as stars");

            showCustomCharInfo = Config.Bind("2 - Character Inventory", nameof(showCustomCharInfo), true, "Show Kills, Deaths, Builds, and Crafts stats on character selection screen");

            showCustomTooltips = Config.Bind("2 - Character Inventory", nameof(showCustomTooltips), true, "Show more info on inventory item tooltips. Disable this is using Epic Loot.");

            showCombinedItemStats = Config.Bind("2 - Character Inventory", nameof(showCombinedItemStats), true, "Show all item stats when mouse is hovered over armor amount.");

            iconScaleSize = Config.Bind("2 - Character Inventory", nameof(iconScaleSize), 0.75f, "Scale item icon by this factor. Ex. 0.75 makes them 75% of original size");


            // Skills UI
            customSkillUI = Config.Bind("3 - Character Skills", nameof(customSkillUI), false, "Toggle the use of custom skills UI (BROKEN)");

            skillUITextSize = Config.Bind("3 - Character Skills", nameof(skillUITextSize), 14, "Select text size on skills UI");


            // Hover Text
            timeLeftStyleFermenter = Config.Bind("4 - Hover Text", nameof(timeLeftStyleFermenter), 2, "Select duration display. 0 = Default, 1 = % Done, 2 = min:sec left");

            timeLeftStylePlant = Config.Bind("4 - Hover Text", nameof(timeLeftStylePlant), 2, "Select duration display. 0 = Default, 1 = % Done, 2 = min:sec left");

            timeLeftStyleCookingStation = Config.Bind("4 - Hover Text", nameof(timeLeftStyleCookingStation), 2, "Select duration display. 0 = Default, 1= % Done, 2 = min:sec left");

            chestHasRoomStyle = Config.Bind("4 - Hover Text", nameof(chestHasRoomStyle), 2, "Select how chest emptyness is displayed. 0 = Default | 1 = % | 2 = items / max_room. | 3 = free slots ");


            // Character XP
            showCharacterXP = Config.Bind("5 - Character XP", nameof(showCharacterXP), true, "Enable Character XP. This combines all skill levels to show overall character progress.");

            showCharacterXpBar = Config.Bind("5 - Character XP", nameof(showCharacterXpBar), true, "Show Character XP Bar on the bottom of the screen. Character XP must be enabled.");

            showCharacterXpBar.Value = showCharacterXP.Value && showCharacterXpBar.Value;

            showXPNotifications = Config.Bind("5 - Character XP", nameof(showXPNotifications), true, "Show whenever you gain xp from actions.");

            notificationTextSizeXP = Config.Bind("5 - Character XP", nameof(notificationTextSizeXP), 14, "XP notification font size.");

            extendedXPNotification = Config.Bind("5 - Character XP", nameof(extendedXPNotification), true, "Extend notification with: (xp gained) [current/overall xp]");


            // Enemy HUD
            customEnemyHud = Config.Bind("6 - Enemy HUD", nameof(customEnemyHud), true, "Enable custom enemy HUD changes. If this is set to false, all options in this section will be disabled.");

            useCustomAlertedStatus = Config.Bind("6 - Enemy HUD", nameof(useCustomAlertedStatus), true, "Hide the vanilla alerted icons above the enemy health bar and instead change the color of the name based on the alerted status.");

            showEnemyHPText = Config.Bind("6 - Enemy HUD", nameof(showEnemyHPText), true, "Show the text with HP amount on enemies health bar.");

            enemyLvlStyle = Config.Bind("6 - Enemy HUD", nameof(enemyLvlStyle), 0, "Choose how enemy lvl is shown. 0 = Default(stars) | 1 = Prefix before name (Lv. 1) | 2 = Both");

            enemyNameTextSize = Config.Bind("6 - Enemy HUD", nameof(enemyNameTextSize), 14, "Font Size of the Name on the enemy");

            enemyHPTextSize = Config.Bind("6 - Enemy HUD", nameof(enemyHPTextSize), 10, "Font size of the HP text on the enemy health bar.");

            showPlayerHPText = Config.Bind("6 - Enemy HUD", nameof(showPlayerHPText), true, "Show the health numbers on other player's health bar in Multiplayer.");

            playerHPTextSize = Config.Bind("6 - Enemy HUD", nameof(playerHPTextSize), 10, "The size of the font to display on other player's health bar in Multiplayer.");

            bossHPTextSize = Config.Bind("6 - Enemy HUD", nameof(bossHPTextSize), 14, "The size of the font To display on the Boss's health bar.");

            makeTamedHPGreen = Config.Bind("6 - Enemy HUD", nameof(makeTamedHPGreen), true, "Make the health bar for tamed creatures green instead of red.");

            maxShowDistance = Config.Bind("6 - Enemy HUD", nameof(maxShowDistance), 1f, "How far you will see enemy HP Bar. This is an multiplier, 1 = game default. 2 = 2x default");


            // Map
            mapPinScaleSize = Config.Bind("7 - Map", nameof(mapPinScaleSize), 1f, "Scale map pins by this factor. Ex. 1.5 makes the 150% of original size.");

            // Debug
            isDebug = Config.Bind("8 - Debug", nameof(isDebug), false, "Enable debug logging");

            /* =======================
             *         xDataUI
             * =======================
             */
            uiData = Config.Bind("9 - xDataUI", nameof(uiData), "none", "This is your customized UI info. (Edit to none, if having issues with UI)");
            /*
            showXPNotifications = Config.Bind("UI", "ShowXPNotifications", true, "Show when you gain xp from actions.");
            extendedXPNotification = Config.Bind("UI", "extendedXPNotification", true, "Extend notification with: (xp gained) [current/overall xp]");
            notificationTextSize = Config.Bind("UI", "notificationTextSize", 14, "Edit XP notification font size.");
            customSkillUI = Config.Bind("UI", "useCustomSkillUI", true, "Toggle the use of custom skills UI");
            skillUITextSize = Config.Bind("UI", "skillUITextSize", 14, "Select text size on skills UI");
            showCustomCharInfo = Config.Bind("UI", "showCustomCharInfo", true, "Toggle the visibility of custom info on character selection");
            showCombinedItemStats = Config.Bind("UI", "showCombinedItemStats", true, "Show all item stats when mouse is hovered over armour amount.");
            timeLeftStyleFermenter = Config.Bind("UI", "timeLeftStyleFermenter", 2, "Select duration display. 0 = Default, 1 = % Done, 2 = min:sec left");
            timeLeftStylePlant = Config.Bind("UI", "timeLeftStylePlant", 2, "Select duration display. 0 = Default, 1 = % Done, 2 = min:sec left");
            timeLeftStyleCookingStation = Config.Bind("UI", "timeLeftStyleCookingStation", 2, "Select duration display. 0 = Default, 1= % Done, 2 = min:sec left");
            chestHasRoomStyle = Config.Bind("UI", "chestHasRoomStyle", 2, "Select how chest emptyness is displayed. 0 = Default | 1 = % | 2 = items / max_room. | 3 = free slots ");

            showDurabilityColor = Config.Bind("Item", "ShowDurabilityColor", true, "Show colored durability bars");
            showItemStars = Config.Bind("Item", "showItemStars", true, "Show item quality as stars");
            showCustomTooltips = Config.Bind("Item", "showCustomTooltips", true, "Show customized tooltips.");
            iconScaleSize = Config.Bind("Item", "ScaleSize", 0.75f, "Scale item icon by this factor. Ex. 0.75 makes them 75% of original size");

            customEnemyHud = Config.Bind("HUD", "useCustomEnemyHud", true, "Toggle the use of custom enemy hud");
            hideEnemyHPText = Config.Bind("HUD", "hideEnemyHPText", false, "Toggle if you want to hide the text with HP amount");
            enemyLvlStyle = Config.Bind("HUD", "enemyLvlStyle", 1, "Choose how enemy lvl is shown. 0 = Default(stars) | 1 = Prefix before name (Lv. 1) | 2 = Both");
            enemyHudTextSize = Config.Bind("HUD", "enemyHudTextSize", 14, "Select Text size on enemyHud");
            maxShowDistance = Config.Bind("HUD", "MaxShowDistance", 1f, "How far you will see enemy HP Bar. This is an multiplier, 1 = game default. 2 = 2x default");
            mapPinScaleSize = Config.Bind("HUD", "mapPinSize", 1f, "Scale map pins by this factor. Ex. 1.5 makes the 150% of original size.");
            */
        }
        public void Start()
        {
            harmony.PatchAll(assembly);
        }
        /*
        public void OnDestroy()
        {
          harmony?.UpatchSelf();
        }
        */
    }
}
