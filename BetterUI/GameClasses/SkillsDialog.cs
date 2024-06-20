using HarmonyLib;
using TMPro;
using UnityEngine.UI;

namespace BetterUI.GameClasses
{
    [HarmonyPatch(typeof(SkillsDialog), nameof(SkillsDialog.Setup))]
    static class SkillsDialogSetupPatch
    {
        static void Postfix(SkillsDialog __instance, Player player)
        {
            if (__instance == null || player == null)
            {
                BetterUI.Main.log.LogError("SkillsDialog or Player instance is null.");
                return;
            }

            try
            {
                __instance.gameObject?.SetActive(true);
            }
            catch
            {
                BetterUI.Main.log.LogError("Failed to set SkillsDialog active.");
            }

            if (Main.showCharacterXP.Value)
            {
                TMP_Text topicText = Utils.FindChild(__instance.transform, "topic")?.GetComponent<TMP_Text>();
                if (topicText != null)
                {
                    topicText.text = $"Level: {Patches.XP.level:0}      Progress: {Patches.XP.LevelProgressPercentage:0%}";
                }
                else
                {
                    BetterUI.Main.log.LogError("Topic text component is null.");
                }
            }

            if (Main.customSkillUI.Value)
            {
                Patches.SkillUI.UpdateDialog(__instance, player);
            }
        }
    }
}