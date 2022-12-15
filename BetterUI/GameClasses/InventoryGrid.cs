using BetterUI.Patches;
using HarmonyLib;

namespace BetterUI.GameClasses
{
    [HarmonyPatch]
    public static class BetterInventoryGrid
    {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(InventoryGrid), "UpdateGui")]
        private static void PatchInventory(ref InventoryGrid __instance, ref Player player, ItemDrop.ItemData dragItem)
        {
            foreach (ItemDrop.ItemData itemData in __instance.m_inventory.GetAllItems())
            {
                InventoryGrid.Element element = __instance.GetElement(itemData.m_gridPos.x, itemData.m_gridPos.y, __instance.m_inventory.GetWidth());

                // would be better if this could be done reliably in an Awake/ Start, but I'm not in the mood to look for one
                if (element.m_icon.gameObject.GetComponent<ItemIconUpdater>() == null)
                {
                    var origScaleComp = element.m_icon.gameObject.AddComponent<ItemIconUpdater>();
                    origScaleComp.Setup(element.m_icon);
                }

                ElementHelper.UpdateElement(element.m_durability, element.m_icon, itemData);

                // Change item quality info (HotKeyBar doesn't do this, so we don't add it to UpdateElement)
                if (Main.showItemStars.Value)
                {
                    if (itemData.m_shared.m_maxQuality > 1)
                    {
                        Stars.Draw(element, itemData.m_quality);
                    }
                }
            }
        }
    }
}