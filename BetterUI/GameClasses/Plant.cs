using HarmonyLib;

namespace BetterUI.GameClasses
{
  [HarmonyPatch]
  static class BetterPlant
  {
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Plant), "GetHoverText")]
    private static bool GetHoverText(Plant __instance, ref string __result)
    {
			if (Main.timeLeftHoverTextPlant.Value == 0) return true;

      return Patches.HoverText.PatchPlant(__instance, ref __result);
    }
  }
}