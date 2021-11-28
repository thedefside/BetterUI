using HarmonyLib;
using UnityEngine.UI;

namespace BetterUI.GameClasses
{
    [HarmonyPatch]
    public static class BetterSkillsDialog
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(SkillsDialog), "Setup")]
        private static void ChangeGUI(ref SkillsDialog __instance, ref Player player)
        {
            __instance.gameObject.SetActive(true);
            if (Main.showCharacterXP.Value)
            {
                Utils.FindChild(__instance.transform, "topic").GetComponent<Text>().text = $"Level: {Patches.XP.level:0}      Progress: {string.Format("{0:0%}", Patches.XP.LevelProgressPercentage)}";
            }
            if (!Main.customSkillUI.Value) return;

            Patches.SkillUI.UpdateDialog(__instance, player);
        }
    }
}
