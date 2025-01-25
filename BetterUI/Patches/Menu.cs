﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterUI.Patches;

static class CustomWatermark
{
    private static readonly int fontSize = 8;
    private static readonly string colorCode = "#ffff00ff";

    public static void Apply(FejdStartup menu)
    {
        menu.m_versionLabel.text = $"{menu.m_versionLabel.text}\n<size={fontSize}><color={colorCode}>{Main.MODNAME}: " + Main.VERSION + "</color></size>";
    }
}

static class CharacterStats
{
    private static readonly float padding = 150f;
    private static readonly int fontSize = 20; // Beware, padding isn't tied to this! Might break layout

    public static void Show(FejdStartup menu)
    {
        if (menu.m_profiles == null)
        {
            menu.m_profiles = SaveSystem.GetAllPlayerProfiles();
        }

        if (menu.m_profileIndex >= menu.m_profiles.Count)
        {
            menu.m_profileIndex = menu.m_profiles.Count - 1;
        }

        if (menu.m_profileIndex >= 0 && menu.m_profileIndex < menu.m_profiles.Count)
        {
            PlayerProfile playerProfile = menu.m_profiles[menu.m_profileIndex];
            string kills = playerProfile.m_playerStats.m_stats[PlayerStatType.EnemyKills] > 0 ? $"Kills: {playerProfile.m_playerStats.m_stats[PlayerStatType.EnemyKills]}   " : string.Empty;
            menu.m_csName.text = $"{playerProfile.GetName()}\n<size={fontSize}>{kills}Deaths: {playerProfile.m_playerStats[PlayerStatType.Deaths]}   Crafts: {playerProfile.m_playerStats[PlayerStatType.Crafts]}   Builds: {playerProfile.m_playerStats[PlayerStatType.Builds]}</size>";
            menu.m_csName.gameObject.SetActive(true);
            Vector2 startBtnPos = (menu.m_csStartButton.transform as RectTransform).anchoredPosition;
            menu.m_csName.rectTransform.anchoredPosition = new Vector2(menu.m_csName.rectTransform.anchoredPosition.x, startBtnPos.y + padding);
            menu.SetupCharacterPreview(playerProfile);
            return;
        }

        menu.m_csName.gameObject.SetActive(false);
        menu.ClearCharacterPreview();
    }
}