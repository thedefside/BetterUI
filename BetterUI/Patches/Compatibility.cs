using UnityEngine;

namespace BetterUI.Patches
{
  static class Compatibility
  {
    public static class QuickSlotsHotkeyBar
    {
      private static readonly string originalPath = "QuickSlotsHotkeyBar";
      private static readonly string parent = "hudroot";
      public static bool isUsing = false;

      /// <summary>
      /// Unachor the QuickSlotsHotkeyBar so it can be moved during customization(F7).
      /// </summary>
      public static void Unanchor(Hud hud)
      {
        Transform parentTransform = Hud.instance.transform.Find(parent);
        Transform quickSlots = parentTransform.Find(originalPath);

        if (quickSlots)
        {
          isUsing = true;
          //Removes the component stopping us from moving the QuickSlotsHotkeyBar.
          GameObject.Destroy(quickSlots.GetComponent("ConfigPositionedElement"));
          // No rotation for now, as objects are created and edited all the time on runtime...
          /*
          int rot = 90 - (Main.staminaBarRotation.Value / 90 % 4 * 90);
          Vector3 oldRot = quickSlots.localEulerAngles;
          quickSlots.localEulerAngles = new Vector3(0, 0, -rot);

          foreach (Transform child in quickSlots)
          {
            Helpers.DebugLine($"Editing: {child}.\n{child.localEulerAngles}\n{quickSlots.localEulerAngles}");
            child.localEulerAngles = oldRot;
          }
          */
          //quickSlots.SetParent(parentTransform, false);
        }
      }
    }
  }
}
