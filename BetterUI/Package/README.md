[<p align="center"><img width="600" src="https://i.imgur.com/TvcCtRJ.png"></p>](https://valheim.thunderstore.io/package/thedefside/BetterUI_Reforged)


# BetterUI: Reforged for Valheim
This mod updates the game UI with few subtle edits.  
You are able to edit and select what updates you want to use. 

# Support
Join our Discord! https://discord.gg/vSEjCNF48K

# Features

#### Add stats to the character select screen
 - Kills (if IG ever starts counting these)
 - Deaths
 - Crafts
 - Builds

#### Customizable Player HUD
 - Makes player HUD elements movemable
 - Custom Health, Food, Stamina, and Eitr bars that can be rotated and resized
 - Adjust text sizes

#### Customized Iventory Icons
 - Show item durability colors (green/yellow/orange/red) instead of white
 - Show item level in stars instead of numbers
 - Custom tooltips with more info
 - Adjustable Icon scale
 - Show all character stats when hovering the Shield icon

#### Customizable Skills
 - Adds XP level for your character
 - Custom Character level increase notification
 - Custom Skill dialog with more info
 - Notifications for all XP gains

#### Custom Hover Text
 - Fermentor, Cooking Stations, Bee hive, and plants shows either time remaining or % complete
 - Chests can show % filled, filled/total, or just filled

#### Custom Enemy HUD
 - Show Enemy level, stars, or both
 - Show enemy current/max health
 - Show alerted status by changing the name color
 - Show tamed creatures health bar as green
 - Show other players current/max health
 - Customize text size
 - Show your own character's health bar above your head
 - Boss current/max health
 - Change distance enemy HUD appears

#### Custom Map
 - Change map pin size



## Version 2.4.0 Information

- Overhauled general scaling and dimension as well as custom bar rotation system (Goldenrevolver):

    - in addition to being able to rotate the custom bars ingame using the new enum config, you can now also rotate them with the mouse wheel in edit mode while not holding down the modifier key. holding down the modifier key and scrolling still changes the scale for any UI element, but additionally while holding down the modifier key moving the mouse horizontally and vertically now changes the X and Y dimension respectively (X dimension support is completely new)
    - custom bar text now scales based on Y axis only (that means that the text is no longer distorted when changing only one dimension)
    - hotbar and inventory icons now scale ingame through new component
    - improved bar update performance by saving in local variables

 - New features ((Goldenrevolver)

    - added font option for custom bar text and custom food bar duration text
    - added option for bee hive hover text like fermenter hover text
    - added option to not show running exp notifications

- Fix compatibility with ComfyQuickSlots and Odin's QOL (BruceOfTheBow & Goldenrevolver)


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
 - Some config values are not loaded in real time. You must log out and log back in to see the changes.

__Can I hide the Yellow XP Bar at the bottom of the screen?__  
 - Yes you can. Check config for `showCharacterXPBar` and set it to `false`  

__Why are the star icons not showing on enemies anymore?__
 - If you want the stars back, edit _enemyLvlStyle_ in the config.  
    - DefaultStars = vanilla stars only
    - PrefixLevelNumber = level number before name (Lvl 1 Greydwarf) with no stars
    - Both = stars & prefix

__How to turn off a specific hover text edit?__  
 - You want to set the specific hover text option to Disabled.  
 - Example: _timeLeftStyleFermenter = Disabled_

__How do I edit the player HUD?__  
 - Press `ESC` to unlock cursor
 - `F7` | Toggle editing mode  
 - `Mouse Left` | Hold to Drag elements  
 - `Mouse Right` | Toggle editing layer
 - `Mouse Left` + `Mouse Wheel` | Hold and scroll to Rotate Custom Bar
 - `Left Ctrl` + `Mouse Left` + `Mouse Wheel` | Hold to scale width and height equally  
 - `Left Ctrl` + `Mouse Left` + Drag pointer left/right | Scale width
 - `Left Ctrl` + `Mouse Left` + Drag pointer up/down | Scale height

 __How do I reset the Custom HUD positions?__
  - Clear out the value in the uiData field in the config

 Small showcase on editing UI: https://imgur.com/a/w6bRkWs 
 
 Full Changelog: https://github.com/thedefside/BetterUI/blob/main/Changelog.txt