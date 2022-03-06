
# BetterUI: Reforged for Valheim
This mod updates the game UI with few subtle edits.  
You are able to edit and select what updates you want to use. 

# Support
Join our Discord! https://discord.gg/vSEjCNF48K

## Version 2.2.1 Information

 - Added config option to enble showing your own player name/health bar/hp text above your player (redseiko)
 - Performance improvements to the EnemyHud (redseiko)
 - Fixed Custom HUD editing (thedefside)
 - Fixed Custom Skills dialog (thedefside)
 - Don't resize enemy hud heatlh bar if hp text is disabled (thedefside)




Small showcase on editing UI: https://imgur.com/a/w6bRkWs 
 

## Table of Contents
1. [Installation](#Installation-(manual))
2. [FAQ](#FAQ)
3. [Changelog](#Changelog)  

## Installation (manual)

If you are installing this manually, do the following _(You will need Bepinex installed)_

1. Extract the archive into a folder. **Do not extract into the game folder.**
2. Move the contents of `plugins` folder into `<GameDirectory>\Bepinex\plugins`.
3. Run the game, it will generate automatically an configuration file into `<GameDirectory>\Bepinex\config`

## FAQ
__How can I see the Epic Loot info when I hover over an item in my iventory?__
 - In the config file, set `showCustomTooltips = false`

__When I use the config manager to change the config values, nothing happens. Why not?__
 - The config values are not loaded in real time. You must quit the game and restart to see the changes.

__Can I hide the Yellow XP Bar at the bottom of the screen?__  
 - Yes you can. Check config for `showCharacterXPBar` and set it to `false`  

__Why are the star icons not showing on enemies anymore?__
 - If you want the stars back, edit _enemyLvlStyle_ in the config.  
    - 0 = stars  
    - 1 = prefix
    - 2 = stars & prefix

__How to turn off a specific hover text edit?__  
 - You want to set the specific hover text option to 0.  
 - Example: _timeLeftStyleFermenter = 0_

__How do I edit the player HUD?__  
 - Press `ESC` to unlock cursor
 - `F7` | Toggle editing mode  
 - `Mouse Left` | Drag elements  
 - `Mouse Right` | Toggle editing layer  
 - `Left Ctrl` | Hold to allow scale editing  
 - `Mouse Scroll` | Edit scale  

## Changelog
#### 2.1.0
 - Re-organized the config file. Renamed some options and updated the descriptions. It is recommended that you back up your existing file, then remove it so a new one is generated
 - Add config setting to enable writing debug messages to the bepinex log
 - Enemy HUD changes:
	- Add config option to show other player's current and max health numbers in multiplayer
	- Add config options to adjust the font size of Enemy, Boss, and Player health
	- Add config option to change the health bar color for tamed creatures from red to green
	- Add config option to change enemy name color based on alerted status. If disabled vanilla alerted icons show
	- Fixed enemy level number not showing when enemyLvlStyle set to 1 or 2
#### 2.0.5
 - Fixed an issue where Player couldn't jump after turning Character XP off and XP bar on (thanks to Varek!)
#### 2.0.4
 - Fix Hover text on oven (you must point to the outer shell of the oven to see it)
 - Update most custom HUD settings to default to disabled
#### 2.0.3
 - Fix cooking stations not working (nandryshak)
 - Fix enemy health bars (nandryshak)
 - Fix food bar timer being stuck at default value (phtnk)
 - Add config option to hide _just_ the XP bar on the bottom of the screen
 - Add XP level progress % to Skills dialog window
#### 2.0.2
- Fixed issues with QuickSlots disappearing
- Custom elements now use game default rotations.
#### 2.0.1
- Fixed issues with custom elements
- Rotate custom elements via config
- Custom UI support for QuickSlots
#### 2.0.0 
- Revamped config, old config causes issues (suggest deleting old and relaunch game)
- Ability to edit UI elements positions
- Added Custom elements (HP Bar, Stamina Bar, Food Bar)
#### 1.6.4
- Fixed fermenter hover text after game update 0.148.6
- Skill notifications are ingnored on lvl 100 skills
- Chest hovertext now has option to show avaible free slots.
#### 1.6.3
- Container hover text follows original format
- XP Notification customization (text size, extended information)
#### 1.6.2
- Fixed issues with enemyHud draw distance defaulting to smaller than game default
- Skill percentage text is now scaled with skillUITextSize
#### 1.6.1
- Fixed fishing rod causing skills to crash / losing session progress.
- Removed decimals from chest percentage.
#### 1.6.0
- Added custom map pin size (for the 4k gang)
- More hovertext information (Cooking station, chest)
- Fixed issue with XP notification caused crashing
- More modification options on cofig.
#### 1.5.1
- Fixed Player XP Bar scaling issues on > 16:9 resolutions. Please notify if you still have issues.
- Added custom hover text on plant & fermenter.
- Durability now supports Protanopia color palette.
- Other config edits.
#### 1.5.0
- Added custom tooltips
- Updated recipe information. Show clearly what stats are improving when upgrading.
- Show all active item stats by hovering over your armour amount.
#### 1.4.2
- Added skillUITextSize to config (Game still scales them)
- Padding fix on modded items quality stars
- Ability to toggle if chracter stats are visible 
#### 1.4.1
- Hotfix on config values
#### 1.4.0
- Added Character Lvls + XP Bar to track progress
- More options to config
- Bug fixes on spawned items breaking items UI
#### 1.3.1
- Fixed durability bars sometime showing as empty
- Small code optimization
- Updated Icon & README
#### 1.3.0
- Added custom enemy hud
- Editable enemy hud visibility range.
- Ability to scale item image sizes. _Defaulted to 75% of original size._
- **Removed First Person Camera** _(There is an seperate mod for that, few updates coming soon.)_
#### 1.2.0
- Changed item levels to display as stars
- Added Character stats to character selection screen
- Minor tweaks on skill UI.
#### 1.1.0
- Added Colored Durability Bars
- Added options to disable UI modifications
- Added config options to edit FPS Toggle and FOV

#### 1.0.1
- Initial Release under Valheim Mods
#### 1.0.0
- Initial Release, but went under the wrong section
