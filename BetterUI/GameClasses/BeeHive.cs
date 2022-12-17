using HarmonyLib;
using System.Configuration;

namespace BetterUI.GameClasses
{
    [HarmonyPatch]
    internal class BetterBeeHive
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(Beehive), "GetHoverText")]
        private static bool GetHoverText(Beehive __instance, ref string __result)
        {
            if (Main.timeLeftHoverTextBeeHive.Value == 0)
            {
                return true;
            }

            if (!PrivateArea.CheckAccess(__instance.transform.position, 0f, false, false))
            {
                __result = Localization.instance.Localize(__instance.m_name + "\n$piece_noaccess");
                return false;
            }

            Patches.HoverText.PatchBeeHive(__instance, ref __result);
            return false;
        }
    }
}