using BetterUI.Patches;
using HarmonyLib;

namespace BetterUI.GameClasses
{
    [HarmonyPatch]
    public static class BetterHotkeyBar
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(HotkeyBar), "UpdateIcons")]
        private static void PatchHotkeyBar(ref HotkeyBar __instance, ref Player player)
        {
            if (!player || player.IsDead())
            {
                return;
            }

            HotkeyBar.ElementData element;
            float xPos;

            foreach (ItemDrop.ItemData itemData in __instance.m_items)
            {
                element = null;
                xPos = itemData.m_gridPos.x;

                if (xPos >= 0 && xPos < __instance.m_elements.Count)
                {
                    element = __instance.m_elements[itemData.m_gridPos.x];
                }

                if (element == null)
                {
                    return;
                }

                // would be better if this could be done reliably in an Awake/ Start, but I'm not in the mood to look for one
                if (element.m_icon.gameObject.GetComponent<ItemIconUpdater>() == null)
                {
                    ItemIconUpdater origScaleComp = element.m_icon.gameObject.AddComponent<ItemIconUpdater>();
                    origScaleComp.Setup(element.m_icon);
                }

                ElementHelper.UpdateElement(element.m_durability, element.m_icon, itemData);
            }
        }
    }
}