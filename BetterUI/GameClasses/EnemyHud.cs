using HarmonyLib;

using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace BetterUI.GameClasses
{
    [HarmonyPatch(typeof(EnemyHud))]
    public static class BetterEnemyHud
    {
        
        private const string BossHpPrefix = "BU_bossHPText";
        private const string EnemyHpPrefix = "BU_enemyHpText";
        private const string PlayerHpPrefix = "BU_playerHPText";

        public static readonly float maxDrawDistance = 3f;

        [HarmonyPostfix]
        [HarmonyPatch(nameof(EnemyHud.Awake))]
        private static void AwakePostfix(ref EnemyHud __instance)
        {
            float limiter = Mathf.Min(Mathf.Abs(Main.maxShowDistance.Value), maxDrawDistance);
            __instance.m_maxShowDistance *= limiter;
        }

        static readonly ConditionalWeakTable<EnemyHud.HudData, TMPro.TextMeshProUGUI> _hpTextCache = new();

        [HarmonyPrefix]
        [HarmonyPatch(nameof(EnemyHud.ShowHud))]
        static void ShowHudPrefix(ref EnemyHud __instance, ref Character c, ref bool __state) {
          __state = __instance.m_huds.ContainsKey(c);
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(EnemyHud.ShowHud))]
        static void ShowHudPostfix(ref EnemyHud __instance, ref Character c, ref bool __state)
        {
            if (!Main.customEnemyHud.Value
                || __state
                || !__instance.m_huds.TryGetValue(c, out EnemyHud.HudData hudData)) {
              return;
            }

            RectTransform hpRoot = (hudData.m_gui.transform.Find("Health") as RectTransform);

            if (c.IsPlayer())
            {
                if (Main.showPlayerHPText.Value)
                {
                    TextMeshProUGUI hpText = UnityEngine.Object.Instantiate(hudData.m_name, hudData.m_name.transform.parent);
                    hpText.name = PlayerHpPrefix;
                    hpText.rectTransform.anchoredPosition = new Vector2(hpText.rectTransform.anchoredPosition.x, 3.5f);
                    hpText.text = $"<size={Main.playerHPTextSize.Value}>{hudData.m_character.GetHealth():0}/{hudData.m_character.GetMaxHealth():0}</size>";
                    hpText.color = Color.white;
                    UnityEngine.Object.Destroy(hpText.GetComponent<Outline>());

                    _hpTextCache.Add(hudData, hpText);

                    hpRoot.sizeDelta = new Vector2(hpRoot.sizeDelta.x, hpRoot.sizeDelta.y * 3f);
                    hudData.m_healthFast.m_bar.sizeDelta = new Vector2(hudData.m_healthFast.m_width, hpRoot.sizeDelta.y);
                    hudData.m_healthSlow.m_bar.sizeDelta = new Vector2(hudData.m_healthSlow.m_width, hpRoot.sizeDelta.y);
                }
            }
            else if (c.IsBoss())
            {
                if (Main.showEnemyHPText.Value)
                {
                    // Edits to Boss HP Bar
                    TextMeshProUGUI hpText = UnityEngine.Object.Instantiate(hudData.m_name, hudData.m_name.transform.parent);
                    hpText.name = BossHpPrefix;
                    hpText.rectTransform.anchoredPosition = new Vector2(hpText.rectTransform.anchoredPosition.x, 0.0f); // orig.y = 21f
                    hpText.text = $"<size={Main.bossHPTextSize.Value}>{hudData.m_character.GetHealth():0} / {hudData.m_character.GetMaxHealth():0}</size>";
                    hpText.color = Color.white;
                    UnityEngine.Object.Destroy(hpText.GetComponent<Outline>());

                    _hpTextCache.Add(hudData, hpText);
                }
            }
            else
            {
                hudData.m_name.fontSize = Main.enemyNameTextSize.Value;
                if (Main.enemyLevelStyle.Value != 0)
                {
                    hudData.m_name.text = hudData.m_name.text.Insert(0, $"<size={Main.enemyNameTextSize.Value}><color=#ffffffff>Lv.{c.m_level} </color></size> ");
                }
                if (Main.showEnemyHPText.Value)
                {

                    hpRoot.sizeDelta = new Vector2(hpRoot.sizeDelta.x, hpRoot.sizeDelta.y * 3f);
                    TextMeshProUGUI hpText = UnityEngine.Object.Instantiate(hudData.m_name, hudData.m_name.transform.parent);
                    hpText.name = EnemyHpPrefix;
                    hpText.rectTransform.anchoredPosition = new Vector2(hpText.rectTransform.anchoredPosition.x, 7.0f); // orig.y = 21f
                    hpText.text = $"<size={Main.enemyHPTextSize.Value}>{hudData.m_character.GetHealth():0}/{hudData.m_character.GetMaxHealth():0}</size>";
                    hpText.color = Color.white;
                    UnityEngine.Object.Destroy(hpText.GetComponent<Outline>());

                    _hpTextCache.Add(hudData, hpText);
                }
                
                if (c.IsTamed() && Main.makeTamedHPGreen.Value)
                {
                    hudData.m_healthFast.SetColor(Color.green);
                    hudData.m_healthSlow.SetColor(Color.green);
                }

                // Resize and position everything
                if (Main.useCustomAlertedStatus.Value)
                {
                    hudData.m_alerted.gameObject.SetActive(false);
                    hudData.m_aware.gameObject.SetActive(false); 
                }
                hudData.m_healthFast.m_bar.sizeDelta = new Vector2(hudData.m_healthFast.m_width, hpRoot.sizeDelta.y);
                hudData.m_healthSlow.m_bar.sizeDelta = new Vector2(hudData.m_healthSlow.m_width, hpRoot.sizeDelta.y);

                if (Main.enemyLevelStyle.Value == Main.EnemyLevelStyle.PrefixLevelNumber)
                {
                    hudData.m_level2.gameObject.SetActive(false);
                    hudData.m_level3.gameObject.SetActive(false);
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(EnemyHud.UpdateHuds))]
        static void UpdateHudsPostfix(ref EnemyHud __instance)
        {
            
            if (!Main.customEnemyHud.Value) return;

            Character character = null;
            foreach (KeyValuePair<Character, EnemyHud.HudData> keyValuePair in __instance.m_huds)
            {
                EnemyHud.HudData value = keyValuePair.Value;
                
                if (!value.m_character || !__instance.TestShow(value.m_character, true))
                {
                    if (character == null)
                    {
                        character = value.m_character;
                        UnityEngine.Object.Destroy(value.m_gui);
                    }
                }
                else
                {
                    if (value.m_character.IsPlayer())
                    {
                        if (Main.showPlayerHPText.Value && _hpTextCache.TryGetValue(value, out TextMeshProUGUI hpText)) {
                          hpText.text = $"<size={Main.playerHPTextSize.Value}>{value.m_character.GetHealth():0}/{value.m_character.GetMaxHealth():0}</size>";
                        }
                    }
                    else if (value.m_character.IsBoss())
                    {
                        if (Main.showEnemyHPText.Value && _hpTextCache.TryGetValue(value, out TextMeshProUGUI hpText)) {
                          hpText.text = $"<size={Main.bossHPTextSize.Value}>{value.m_character.GetHealth():0} / {value.m_character.GetMaxHealth():0}</size>";
                        }
                    }
                    else
                    {
                        if (Main.useCustomAlertedStatus.Value)
                        {
                            value.m_alerted.gameObject.SetActive(false);
                            value.m_aware.gameObject.SetActive(false);
                            bool aware = value.m_character.GetBaseAI().HaveTarget();
                            bool alerted = value.m_character.GetBaseAI().IsAlerted();
                            value.m_name.color = (aware || alerted) ? (alerted ? Color.red : Color.yellow) : Color.white;
                        }
                                   

                        if (Main.showEnemyHPText.Value && _hpTextCache.TryGetValue(value, out TextMeshProUGUI hpText)) {
                          hpText.text = $"<size={Main.enemyHPTextSize.Value}>{value.m_character.GetHealth():0}/{value.m_character.GetMaxHealth():0}</size>";
                        }

                        if (Main.enemyLevelStyle.Value != 0)
                        {
                            value.m_name.text = value.m_name.text.Insert(0, $"<size={Main.enemyNameTextSize.Value}><color=#ffffffff>Lv.{value.m_character.m_level} </color></size> ");
                        }

                        if (Main.enemyLevelStyle.Value == Main.EnemyLevelStyle.PrefixLevelNumber)
                        {
                            value.m_level2.gameObject.SetActive(false);
                            value.m_level3.gameObject.SetActive(false);
                        }
                    }
                }
                
            }
            if (character != null)
            {
                __instance.m_huds.Remove(character);
            }
             
        }

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(EnemyHud.LateUpdate))]
        static IEnumerable<CodeInstruction> LateUpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
          return new CodeMatcher(instructions)
              .MatchForward(
                  useEnd: false,
                  new CodeMatch(OpCodes.Stloc_3),
                  new CodeMatch(OpCodes.Ldloc_3),
                  new CodeMatch(OpCodes.Ldloc_1),
                  new CodeMatch(OpCodes.Call))
              .Advance(offset: 3)
              .SetInstructionAndAdvance(
                  Transpilers.EmitDelegate<Func<Character, Player, bool>>(CharacterLocalPlayerEqualityDelegate))
              .InstructionEnumeration();
        }

        static bool CharacterLocalPlayerEqualityDelegate(Character character, Player player) {
          if (Main.showLocalPlayerEnemyHud.Value) {
            return false;
          }

          return character == player;
        }

        public static void ShowLocalPlayerEnemyHudConfigChanged() {
          if (Player.m_localPlayer
              && EnemyHud.m_instance
              && EnemyHud.m_instance.m_huds.TryGetValue(Player.m_localPlayer, out EnemyHud.HudData hudData)) {
              UnityEngine.Object.Destroy(hudData.m_gui);
              EnemyHud.m_instance.m_huds.Remove(Player.m_localPlayer);
          }
        }
  }

}
