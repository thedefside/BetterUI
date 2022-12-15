using UnityEngine;
using UnityEngine.UI;

namespace BetterUI.Patches
{
    internal class ItemIconUpdater : MonoBehaviour
    {
        private Vector3 origScale;
        private Image icon;

        public void Setup(Image icon)
        {
            this.icon = icon;
            this.origScale = icon.transform.localScale;
            IconScaleSize_SettingChanged();

            Main.iconScaleSize.SettingChanged += (_, _) => IconScaleSize_SettingChanged();
        }

        private void IconScaleSize_SettingChanged()
        {
            icon.transform.localScale = origScale * Mathf.Max(Main.iconScaleSize.Value, 0.1f);
        }
    }
}