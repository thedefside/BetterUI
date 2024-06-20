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
            int width = __instance.m_inventory.GetWidth();
            InventoryGrid.Element element;
            int index;

            foreach (ItemDrop.ItemData itemData in __instance.m_inventory.GetAllItems())
            {
                index = itemData.m_gridPos.y * width + itemData.m_gridPos.x;
                element = null;

                if (index >= 0 && index < __instance.m_elements.Count)
                {
                    element = __instance.m_elements[index];
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