<div align="center">
  <img src="Resources/icon256.ico" alt="App Icon" width="124" style="display:inline; vertical-align:middle;">

  # Terrors of Nowhere: Save Manager
  Simple tool that will keep track of your **Save Codes** so you can play and recover your codes later if you forgot to make a backup.
  And saves your code history locally for later use.

  # [Download](https://github.com/ChrisFeline/ToNSaveManager/releases/latest/download/ToNSaveManager.zip "Use this link to download the latest version directly from GitHub.")
  
  ### [Discord Server](https://discord.gg/Anpm8d3fPD) • [VRChat Group](https://vrc.group/TONSM.0849)

  [View Releases](https://github.com/ChrisFeline/ToNSaveManager/releases "Show a list of current and previous releases.") • 
  [Saving Guide](https://terror.moe/save "HOW TO SAVE & LOAD FOR DUMMIES") • 
  [FAQ](Docs/FAQ.md)
</div>

<p align="center">
  <img src="Resources/preview.png" alt="Preview" title="Boo!">
</p>

# 🛠️ Features & Clarifications
- Automatically scans your logs for previous **Save Codes**.
- While the tool is running, it will detect new codes as you play.
- Previously detected save codes will be saved to a local database, so if VRChat deletes logs overtime, you'll have a history of Save Codes locally, and safe.

## Settings Window
- `Check For Updates` When clicked, it will check this github repo for new releases, and prompt you to try an automatic update.
- `Auto Clipboard Copy` Automatically copy new save codes to clipboard.
- `Collect Player Names` Save codes will show players that were in the instance.
- `XSOverlay Popup` XSOverlay notifications when new save codes are detected.
- `Play Sound` Play a notification audio when a new save is detected.
  - Double Click to select a custom audio file. (Only '.wav' files)
  - Right Click to reset audio file back to 'default.wav'
- `Colorful Objectives` Items in the 'Objectives' window will show colors that correspond to those of the items in the game.
- `Auto Discord Backup` Uses a [discord webhook](#how-to-properly-configure-automatic-discord-backup-using-webhooks) to automatically upload a backup of your new codes to a discord channel as you play.
- `Send OSC Parameters` Sends avatar parameters to VRChat using OSC. Check the [documentation](#osc-documentation) below for more info.
- `WebSocket API Server` Enables a WebSocket server that sends realtime in-game events to the connected clients. Check the [**API Documentation**](Docs/WebSocketAPI.md) for more info.
- `Send Chatbox Message` Sends ToN info to the VRChat chatbox. (Only visible during Intermissions) - To further customize the template read [**Templates Documentation**](Docs/Templates.md) for more info.
<details><summary>Preview Image</summary><p> <img src="Resources/settings.png" > </p></details>

## Right Click Menus
- ### Log Dates (Left Panel)
  * `Import` You can enter your own code and save it in that collection.
  * `Rename` Lets you rename a collection.
  * `Delete` Deletes the entire log date from the database.
- ### Save Codes (Right Panel)
  * `Add to` Lets you save or favorite this code to a separated custom collection with a name of your choice.
  * `Edit Note` You can attach a note to this save code, so you can recognize it better.
  * `Backup` Forces a backup upload to Discord if **Auto Discord Backup** is configured on settings.
  * `Delete` Deletes just this save code from the database.
  
## Objectives Window
- This window gives you a list of unlockables that you can check to track your progress. Just click on the things you already unlocked.

## OSC Documentation
- [**Parameter Names & Types**](Docs/OSC/OSC_Parameters.md)
- [**Round Type Values**](Docs/OSC/OSC_RoundType.md)

<details><summary>OSC Troubleshooting</summary><p>
If your parameters are not being received properly... try resetting the OSC config.

<p>You can do this by opening your <b>Radial menu</b>, open <b>OSC</b>, then click <b>Reset Config</b>.</p>

<img src="Resources/osc_reset.png" >
</p></details>

## Scripting
- You can extend this tool using JavaScript.
  - Read the [Script Documentation](Docs/Scripts.md) to get started.

# 🌐 Available Translations
> Check [`Localization/CONTRIBUTE.md`](Localization/CONTRIBUTE.md) to help translate the Save Manager to your language.

| Language | Translator |
| -------- | ---------- |
| English  | -          |
| Spanish  | -          |
| Japanese | [github.com/nomlasvrc](https://github.com/nomlasvrc) <br> [twitter.com/nomlasvrc](https://twitter.com/nomlasvrc) |
| German   | [github.com/sageyx2002](https://github.com/sageyx2002) |
| Traditional Chinese  | [github.com/XoF-eLtTiL](https://github.com/XoF-eLtTiL) |
| Simplified Chinese  | [github.com/Fallen-ice](https://github.com/Fallen-ice) |
| Italian  | [github.com/TheIceDragonz](https://github.com/TheIceDragonz) |

# 📫 Contact:
> ### Please do <u>NOT</u> message Beyond about suggestions or problems with this tool.
> You can report problems or suggestions under the [Issues](https://github.com/ChrisFeline/ToNSaveManager/issues) tab on this repo. Alternatively see contact information below.

> - **Discord:** [@Kittenji](https://discord.gg/Anpm8d3fPD)<br>
> - **VRChat:** [Kittenji](https://vrchat.com/home/user/usr_7ac745b8-e50e-4c9c-95e5-8e7e3bcde682)
> ## Say hi if you see me playing [Terrors of Nowhere](https://vrchat.com/home/world/wrld_a61cdabe-1218-4287-9ffc-2a4d1414e5bd)!
> <p> <img src="Resources/loop.gif" alt="Preview" title="AAAAAA!"> </p>

# ❤️ Support:
> If you want to support the development of this tool you can [Buy Me A Coffee ♥](https://ko-fi.com/kittenji) on ko-fi.